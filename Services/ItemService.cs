using GrpcService1.Data;
using GrpcService1.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcService1.Services;

public class ItemService : IItemService
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;

    public ItemService(ApplicationDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<IEnumerable<UserItem>> GetUserItemsAsync(int userId)
    {
        return await _context.UserItems
            .Include(up => up.Item)
            .Where(up => up.UserId == userId)
            .ToListAsync();
    }

    public async Task<(bool success, decimal newBalance)> SellItemAsync(int userId, int userItemId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var userItem = await _context.UserItems
                .Include(up => up.Item)
                .FirstOrDefaultAsync(up => up.Id == userItemId && up.UserId == userId);

            if (userItem == null) return (false, 0);

            // Remove the Item from user's inventory
            _context.UserItems.Remove(userItem);
            
            // Add the value to user's balance
            await _userService.AddBalanceAsync(userId, userItem.Item.Value);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            var user = await _userService.GetByIdAsync(userId);
            return (true, user.Balance);
        }
        catch
        {
            await transaction.RollbackAsync();
            return (false, 0);
        }
    }
}