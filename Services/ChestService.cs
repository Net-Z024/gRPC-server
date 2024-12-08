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
            .Include(c => c.PossibleProducts)
            .ThenInclude(cp => cp.Product)
            .ToListAsync();
    }

    public async Task<Product> OpenChestAsync(int userId, int chestId)
    {
        var chest = await _context.Chests
            .Include(c => c.PossibleProducts)
            .ThenInclude(cp => cp.Product)
            .FirstOrDefaultAsync(c => c.Id == chestId);

        if (chest == null) throw new ArgumentException("Chest not found");

        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new ArgumentException("User not found");

        if (!await _userService.SpendBalanceAsync(userId, chest.Price))
            throw new InvalidOperationException("Insufficient balance");

        // Draw product based on drop chances
        var roll = (decimal)_random.NextDouble();
        decimal cumulative = 0;
        
        foreach (var chestProduct in chest.PossibleProducts)
        {
            cumulative += chestProduct.DropChance;
            if (roll <= cumulative)
            {
                user.AddProduct(chestProduct.Product);
                await _context.SaveChangesAsync();
                return chestProduct.Product;
            }
        }

        throw new InvalidOperationException("No product was drawn (sum of drop chances might be less than 1)");
    }
}