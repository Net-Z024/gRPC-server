using System.Linq.Expressions;
using GrpcService1.Data;
using GrpcService1.Models;
using GrpcService1.Services;
using GrpcService1.Tests.TestHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Xunit;

public class ChestServiceTests
{
    private readonly Mock<ApplicationDbContext> _contextMock;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<DbSet<Chest>> _chestDbSetMock;
    private readonly Mock<DbSet<User>> _userDbSetMock;
    private readonly ChestService _chestService;
    private readonly List<Chest> _chests;
    private readonly List<User> _users;

    public ChestServiceTests()
    {
        _chests = new List<Chest>();
        _users = new List<User>();

        // Setup mock DbSets with async support
        _chestDbSetMock = SetupMockDbSet(_chests);
        _userDbSetMock = SetupMockDbSet(_users);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _contextMock = new Mock<ApplicationDbContext>(options);
        _contextMock.Setup(x => x.Chests).Returns(_chestDbSetMock.Object);
        _contextMock.Setup(x => x.Users).Returns(_userDbSetMock.Object);

        // Setup the Include chain
        var mockIncludableChests = new Mock<IIncludableQueryable<Chest, object>>();
        mockIncludableChests.As<IQueryable<Chest>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<Chest>(_chests.AsQueryable().Provider));
        mockIncludableChests.As<IQueryable<Chest>>()
            .Setup(m => m.Expression)
            .Returns(_chests.AsQueryable().Expression);
        mockIncludableChests.As<IQueryable<Chest>>()
            .Setup(m => m.ElementType)
            .Returns(_chests.AsQueryable().ElementType);
        mockIncludableChests.As<IQueryable<Chest>>()
            .Setup(m => m.GetEnumerator())
            .Returns(() => _chests.GetEnumerator());

        // Setup FirstOrDefaultAsync for the includable queryable
        mockIncludableChests.Setup(x => x.FirstOrDefaultAsync(
                It.IsAny<Expression<Func<Chest, bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Chest, bool>> predicate, CancellationToken token) =>
                _chests.FirstOrDefault(predicate.Compile()));

        _chestDbSetMock.Setup(x => x.Include(It.IsAny<Expression<Func<Chest, object>>>()))
            .Returns(mockIncludableChests.Object);
        mockIncludableChests.Setup(x => x.Include(It.IsAny<Expression<Func<Chest, object>>>()))
            .Returns(mockIncludableChests.Object);

        _userServiceMock = new Mock<IUserService>();
        _chestService = new ChestService(_contextMock.Object, _userServiceMock.Object);
    }

    private Mock<DbSet<T>> SetupMockDbSet<T>(List<T> sourceList) where T : class
    {
        var queryable = sourceList.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();

        mockSet.As<IAsyncEnumerable<T>>()
            .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
            .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

        mockSet.As<IQueryable<T>>()
            .Setup(m => m.Provider)
            .Returns(new TestAsyncQueryProvider<T>(queryable.Provider));

        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

        return mockSet;
    }

    [Fact]
    public async Task OpenChest_ShouldThrowException_WhenChestNotFound()
    {
        // Arrange
        _chests.Clear();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => 
            _chestService.OpenChestAsync(1, 1));
    }

    [Fact]
    public async Task OpenChest_ShouldReturnProduct_WhenSuccessful()
    {
        // Arrange
        var product = new Product("Test Product", 10m);
        var chest = new Chest("Test Chest", 100m);
        chest.AddPossibleProduct(product, 1.0m);
        _chests.Add(chest);

        var user = new User("test-identity");
        user.AddBalance(200m);
        _users.Add(user);

        _userServiceMock.Setup(x => x.SpendBalanceAsync(It.IsAny<int>(), It.IsAny<decimal>()))
            .ReturnsAsync(true);

        // Act
        var result = await _chestService.OpenChestAsync(1, 1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
    }
}