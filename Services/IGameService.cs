using GrpcService1.Models;

namespace GrpcService1.Services
{
    public interface IGameService
    {
        Task<Game> CreateGameAsync(int hostId, int caseId, int maxPlayers);
        Task JoinGameAsync(int gameId, int userId);
        Task MarkReadyAsync(int gameId, int userId);
        Task<StartGameResponse> StartGameAsync(int gameId);
        Task<List<Game>> GetOpenGamesAsync();
    }
}
