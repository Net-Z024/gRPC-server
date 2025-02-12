using GrpcService1.Data;
using GrpcService1.Models;
using GrpcService1.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static Google.Rpc.Context.AttributeContext.Types;

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
    public async Task<User> GetByIdentityUserIdAsync(string identityUserId)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.IdentityId == identityUserId);
    }

    public async Task<User> CreateUserByIdentityAsync(string identityUserId)
    {
        var user = new User(identityUserId);

        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
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
    
    public async Task CreateUserAsync(User user)
    {
        user = new User(user.IdentityId);
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await Task.CompletedTask;
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await Task.FromResult(_context.Users.FirstOrDefault(u => u.Id == userId));
    }

    public async Task UpdateUserAsync(User user)
    {
        var existingUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
        if (existingUser != null)
        {
            existingUser.SetBalance(user.Balance);
            await _context.SaveChangesAsync();
        }
        await Task.CompletedTask;
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == userId);
        if (user != null)
        {
            _context.Users.Remove(user);
            await Task.FromResult(true);
            await _context.SaveChangesAsync();
            return true;
        }
        return await Task.FromResult(false);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await Task.FromResult(_context.Users);
    }
}