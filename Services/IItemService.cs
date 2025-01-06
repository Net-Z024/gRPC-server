using GrpcService1.Models;

namespace GrpcService1.Services;

public interface IItemService
{
    Task<IEnumerable<UserItem>> GetUserItemsAsync(int userId);
    Task<IEnumerable<ChestItem>> GetChestItemsAsync(int chestId);

    Task<(bool success, decimal newBalance)> SellItemAsync(int userId, int userItemId);
}