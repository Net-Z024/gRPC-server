using GrpcService1.Data;
using GrpcService1.Models;
using GrpcService1.Services;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> GetByIdAsync(int userId)
    {
        return await _context.Users.FindAsync(userId);
    }

    public async Task<User> CreateUserAsync(int userId)
    {
        var user = new User(userId);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> AddBalanceAsync(int userId, decimal amount)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.AddBalance(amount);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SpendBalanceAsync(int userId, decimal amount)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        if (!user.TrySpendBalance(amount)) return false;

        await _context.SaveChangesAsync();
        return true;
    }
}