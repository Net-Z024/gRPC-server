using GrpcService1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GrpcService1.Services
{
    public interface IChestService
    {
        Task<IEnumerable<Chest>> GetAllChestsAsync();
        Task<Item> OpenChestAsync(int userId, int chestId);
        Task CreateChestAsync(Chest chest);
        Task<Chest> GetChestByIdAsync(int chestId);
        Task UpdateChestAsync(Chest chest);
        Task<bool> DeleteChestAsync(int chestId);
    }
}