using GrpcService1.Data;
using GrpcService1.Models;
using GrpcService1.Services;
using Microsoft.EntityFrameworkCore;

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
            Console.WriteLine("Am i here?");
            cumulative += chestItem.DropChance;
            Console.WriteLine("cumulative :" + cumulative);
            Console.WriteLine("Roll: " + roll);
            if (roll <= cumulative)
            {
                Console.WriteLine("heja");

                user.AddItem(item: chestItem.Item);
                await _context.SaveChangesAsync();
                Console.WriteLine("DOSTALES TO:" +chestItem.Item.Name);
                return chestItem.Item;
            }
        }
        Console.WriteLine("huh"+chest.PossibleItems);
        throw new InvalidOperationException("No Item was drawn (sum of drop chances might be less than 1)");
    }
}