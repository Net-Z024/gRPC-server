using GrpcService1.Models;

namespace GrpcService1.Services;

public interface IProductService
{
    Task<IEnumerable<UserProduct>> GetUserProductsAsync(int userId);
    Task<(bool success, decimal newBalance)> SellProductAsync(int userId, int userProductId);
}