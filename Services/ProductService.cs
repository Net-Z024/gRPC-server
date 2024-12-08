using GrpcService1.Data;
using GrpcService1.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcService1.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IUserService _userService;

    public ProductService(ApplicationDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<IEnumerable<UserProduct>> GetUserProductsAsync(int userId)
    {
        return await _context.UserProducts
            .Include(up => up.Product)
            .Where(up => up.UserId == userId)
            .ToListAsync();
    }

    public async Task<(bool success, decimal newBalance)> SellProductAsync(int userId, int userProductId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var userProduct = await _context.UserProducts
                .Include(up => up.Product)
                .FirstOrDefaultAsync(up => up.Id == userProductId && up.UserId == userId);

            if (userProduct == null) return (false, 0);

            // Remove the product from user's inventory
            _context.UserProducts.Remove(userProduct);
            
            // Add the value to user's balance
            await _userService.AddBalanceAsync(userId, userProduct.Product.Value);

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