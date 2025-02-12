using GrpcService1.Data;
using GrpcService1.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcService1.Services;

public class ChestService : IChestService
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;
    private readonly Random _random;

    public ChestService(ApplicationDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
        _random = new Random();
    }

    public async Task<IEnumerable<Chest>> GetAllChestsAsync()
    {
        return await _context.Chests
            .Include(c => c.PossibleItems)
            .ThenInclude(cp => cp.Item)
            .ToListAsync();
    }

    public async Task<Item> OpenChestAsync(int userId, int chestId)
    {
        var chest = await _context.Chests
            .Include(c => c.PossibleItems)
            .ThenInclude(cp => cp.Item)
            .FirstOrDefaultAsync(c => c.Id == chestId);

        if (chest == null) throw new ArgumentException("Chest not found");

        var user = await _context.Users
            .Include(u => u.UserItems)
            .FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new ArgumentException("User not found");

        if (!await _userService.SpendBalanceAsync(userId, chest.Price))
            throw new InvalidOperationException("Insufficient balance");

        var roll = (decimal)_random.NextDouble();
        decimal cumulative = 0;

        foreach (var chestItem in chest.PossibleItems)
        {
            cumulative += chestItem.DropChance;
            if (roll <= cumulative)
            {
                user.AddItem(chestItem.Item);
                await _context.SaveChangesAsync();
                return chestItem.Item;
            }
        }

        throw new InvalidOperationException("No Item was drawn (sum of drop chances might be less than 1)");
    }

    public async Task CreateChestAsync(Chest? chest)
    {
        await _context.Chests.AddAsync(chest);
        await _context.SaveChangesAsync();
    }

    public async Task<Chest?> GetChestByIdAsync(int chestId)
    {
        return await _context.Chests!
            .Include<Chest, ICollection<ChestItem>>(c => c!.PossibleItems)
            .ThenInclude(cp => cp.Item)
            .FirstOrDefaultAsync(c => c.Id == chestId);
    }

    public async Task UpdateChestAsync(Chest chest)
    {
        var existingChest = await _context.Chests
            .Include(c => c.PossibleItems)
            .FirstOrDefaultAsync(c => c.Id == chest.Id);

        if (existingChest == null) throw new ArgumentException("Chest not found");

        existingChest.Update(chest.Name, chest.Price, chest.PossibleItems);
        existingChest.ClearPossibleItems();
        foreach (var chestItem in chest.PossibleItems)
        {
            existingChest.AddPossibleItem(chestItem.ItemId, chestItem.DropChance);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteChestAsync(int chestId)
    {
        var chest = await _context.Chests.FirstOrDefaultAsync(c => c.Id == chestId);
        if (chest == null) return false;

        _context.Chests.Remove(chest);
        await _context.SaveChangesAsync();
        return true;
    }
}