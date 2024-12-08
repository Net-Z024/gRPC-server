using GrpcService1.Models;

namespace GrpcService1.Services;

// Services/IChestService.cs
public interface IChestService
{
    Task<IEnumerable<Chest>> GetAllChestsAsync();
    Task<Product> OpenChestAsync(int userId, int chestId);
}