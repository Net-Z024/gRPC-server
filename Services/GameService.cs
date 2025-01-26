using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrpcService1.Data;
using GrpcService1.Models;
using Microsoft.EntityFrameworkCore;

namespace GrpcService1.Services
{
    public class GameService : IGameService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        public GameService(ApplicationDbContext context,IUserService userService)
        {
            _context = context;
            _userService = userService;
        }
        public async Task<List<Game>> GetOpenGamesAsync()
        {
            return await _context.Games
                .Include(g => g.GamePlayers)
                .Where(g => !g.isStarted && g.GamePlayers.Count < g.maxPlayers)
                .ToListAsync();
        }

        public async Task<Game> CreateGameAsync(int hostId, int caseId, int maxPlayers)
        {
            var user = await _context.Users.FindAsync(hostId);
            var selectedCase = await _context.Chests.Include(c => c.PossibleItems).FirstOrDefaultAsync(c => c.Id == caseId);

            if (user == null || selectedCase == null)
                throw new Exception("Invalid user or case.");

            if (user.Balance < selectedCase.Price)
                throw new Exception("Insufficient balance.");

            var game = new Game
            {
                hostId = hostId,
                caseId = caseId,
                maxPlayers = maxPlayers,
                isStarted = false,
                createdAt = DateTime.UtcNow
            };

            _context.Games.Add(game);
            await _context.SaveChangesAsync();

            var gamePlayer = new GamePlayer
            {
                gameId = game.Id,
                userId = hostId,
                isReady = false
            };

            _context.GamePlayers.Add(gamePlayer);
            await _context.SaveChangesAsync();

            return game;
        }

        public async Task JoinGameAsync(int gameId, int userId)
        {
            var game = await _context.Games.Include(g => g.GamePlayers).FirstOrDefaultAsync(g => g.Id == gameId);
            var user = await _context.Users.FindAsync(userId);

            if (game == null || user == null)
                throw new Exception("Invalid game or user.");

            if (game.isStarted || game.GamePlayers.Count >= game.maxPlayers)
                throw new Exception("Game is full or has already started.");

            bool responseBalance = await _userService.SpendBalanceAsync(userId, game.chest.Price);

            var gamePlayer = new GamePlayer
            {
                gameId = gameId,
                userId = userId,
                isReady = false
            };

            _context.GamePlayers.Add(gamePlayer);
            await _context.SaveChangesAsync();
        }

        public async Task MarkReadyAsync(int gameId, int userId)
        {
            var player = await _context.GamePlayers.FirstOrDefaultAsync(p => p.gameId == gameId && p.userId == userId);

            if (player == null)
                throw new Exception("Player not found in game.");

            player.isReady = true;
            await _context.SaveChangesAsync();
        }

        public async Task<StartGameResponse> StartGameAsync(int gameId)
        {
            var game = await _context.Games
                .Include(g => g.GamePlayers)
                .Include(g => g.chest.PossibleItems)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null || !game.GamePlayers.All(p => p.isReady))
                throw new Exception("Not all players are ready.");

            var random = new Random();
            var results = new List<PlayerResult>();
            foreach (var player in game.GamePlayers)
            {
                var item = game.chest.PossibleItems.OrderBy(_ => random.Next()).First();
                results.Add(new PlayerResult
                {
                    UserId = player.userId,
                    ItemName = item.Item.Name,
                    Value = (int)item.Item.Value
                });

                player.spinResult = (int?)item.Item.Value;
            }

            var winner = results.OrderByDescending(r => r.Value).First();
            var winnerPlayer = await _context.Users.FindAsync(winner.UserId);

            var totalPot = game.chest.Price * game.GamePlayers.Count;
            await _userService.AddBalanceAsync(winner.UserId, totalPot);

            game.isStarted = true;
            await _context.SaveChangesAsync();

            return new StartGameResponse
            {
                WinnerId = winner.UserId,
                Results = { results }
            };
        }
    }
}
