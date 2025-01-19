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

    public async Task<User> CreateUserAsync(string identityUserId)
    {
        var user = new User(identityUserId);

        
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