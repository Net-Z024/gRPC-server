using GrpcService1.Data;
using GrpcService1.Models;
using GrpcService1.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace GrpcService1.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly UserService _userService;
        private readonly Mock<DbSet<User>> _userDbSetMock;
        private readonly List<User> _users;

        public UserServiceTests()
        {
            _users = new List<User>();
        
            // Setup mock DbSet
            var queryableUsers = _users.AsQueryable();
            _userDbSetMock = new Mock<DbSet<User>>();
        
            // Setup IQueryable
            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.Provider).Returns(queryableUsers.Provider);
            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.Expression).Returns(queryableUsers.Expression);
            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(queryableUsers.ElementType);
            _userDbSetMock.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(() => queryableUsers.GetEnumerator());
        
            // Setup Find methods specifically for User type
            _userDbSetMock.Setup(x => x.Find(It.IsAny<object[]>()))
                .Returns<object[]>(ids => _users.FirstOrDefault(u => u.Id == (int)ids[0]));
            _userDbSetMock.Setup(x => x.FindAsync(It.IsAny<object[]>()))
                .Returns<object[]>(ids => new ValueTask<User?>(_users.FirstOrDefault(u => u.Id == (int)ids[0])));

            // Setup context
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            _contextMock = new Mock<ApplicationDbContext>(options);
            _contextMock.Setup(x => x.Users).Returns(_userDbSetMock.Object);

            _userService = new UserService(_contextMock.Object);
        }

        [Fact]
        public async Task CreateUser_ShouldCreateNewUser_WithCorrectIdentityId()
        {
            // Arrange
            string identityId = "test-identity";
            User savedUser = null;
            _contextMock.Setup(x => x.Users.Add(It.IsAny<User>()))
                .Callback<User>(user => savedUser = user);

            // Act
            var result = await _userService.CreateUserAsync(identityId);

            // Assert
            Assert.NotNull(savedUser);
            Assert.Equal(identityId, savedUser.IdentityId);
            Assert.Equal(0, savedUser.Balance);
        }

        [Fact]
        public async Task AddBalance_ShouldIncreaseBalance_WhenUserExists()
        {
            // Arrange
            var user = new User("test-identity");
            _users.Add(user);
            var initialBalance = user.Balance;
            var addAmount = 100m;

            // Act
            var result = await _userService.AddBalanceAsync(1, addAmount);

            // Assert
            Assert.True(result);
            Assert.Equal(initialBalance + addAmount, user.Balance);
        }

        [Fact]
        public async Task SpendBalance_ShouldReturnFalse_WhenInsufficientBalance()
        {
            // Arrange
            var user = new User("test-identity");
            user.AddBalance(50m);
            _users.Add(user);

            // Act
            var result = await _userService.SpendBalanceAsync(1, 100m);

            // Assert
            Assert.False(result);
            Assert.Equal(50m, user.Balance);
        }
    }
}