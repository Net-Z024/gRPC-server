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

    public async Task<IEnumerable<ChestItem>> GetChestItemsAsync(int chestId)
    {
        return await _context.ChestItems
            .Include(ci => ci.Item)
            .Where(ci => ci.ChestId == chestId)
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

            _context.UserItems.Remove(userItem);
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

    // IItemService Methods

    public async Task CreateItemAsync(Item item)
    {
        await _context.Items.AddAsync(item);
        await _context.SaveChangesAsync();
    }

    public async Task<Item> GetItemByIdAsync(int id)
    {
        return await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task UpdateItemAsync(Item item)
    {
        var existingItem = await _context.Items.FirstOrDefaultAsync(i => i.Id == item.Id);
        if (existingItem == null)
        {
            throw new ArgumentException("Item not found");
        }

        existingItem.Update(item.Name, item.Value, item.ImageUrl);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteItemAsync(int id)
    {
        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
        if (item == null)
        {
            return false;
        }

        _context.Items.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Item>> GetAllItemsAsync()
    {
        return await _context.Items.ToListAsync();
    }
}
