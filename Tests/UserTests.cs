using GrpcService1.Models;
using Xunit;

namespace GrpcService1.Tests
{
    public class UserTests
    {
        [Fact]
        public void AddBalance_ShouldIncreaseBalance()
        {
            // Arrange
            var user = new User("test-identity");
            var initialBalance = user.Balance;
            var addAmount = 100m;

            // Act
            user.AddBalance(addAmount);

            // Assert
            Assert.Equal(initialBalance + addAmount, user.Balance);
        }

        [Fact]
        public void AddBalance_ShouldThrowException_WhenAmountIsNegative()
        {
            // Arrange
            var user = new User("test-identity");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => user.AddBalance(-100m));
        }

        [Theory]
        [InlineData(100, 50, true)]  // Has enough balance
        [InlineData(50, 100, false)] // Not enough balance
        [InlineData(0, 1, false)]    // No balance
        public void TrySpendBalance_ShouldWorkCorrectly(decimal initialBalance, decimal spendAmount, bool expectedResult)
        {
            // Arrange
            var user = new User("test-identity");
            user.AddBalance(initialBalance);

            // Act
            var result = user.TrySpendBalance(spendAmount);

            // Assert
            Assert.Equal(expectedResult, result);
            Assert.Equal(
                expectedResult ? initialBalance - spendAmount : initialBalance,
                user.Balance
            );
        }
    }
}