using GrpcService1.Models;

namespace GrpcService1.Services;

public interface IItemService
{
    Task<IEnumerable<UserItem>> GetUserItemsAsync(int userId);
    Task<IEnumerable<ChestItem>> GetChestItemsAsync(int chestId);

    Task<(bool success, decimal newBalance)> SellItemAsync(int userId, int userItemId);
    Task CreateItemAsync(Item item);
    Task<Item> GetItemByIdAsync(int id);
    Task UpdateItemAsync(Item item);
    Task<bool> DeleteItemAsync(int id);
    Task<IEnumerable<Item>> GetAllItemsAsync();
}