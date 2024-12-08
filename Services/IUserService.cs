using GrpcService1.Models;

namespace GrpcService1.Services;

public interface IUserService
{
    Task<User> GetByIdAsync(int userId);
    Task<User> CreateUserAsync(string identityId);
    Task<bool> AddBalanceAsync(int userId, decimal amount);
    Task<bool> SpendBalanceAsync(int userId, decimal amount);
}