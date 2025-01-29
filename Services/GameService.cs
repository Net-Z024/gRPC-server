using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
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
            var activeGame = await _context.Games.FirstOrDefaultAsync(g => g.hostId == hostId && !g.isStarted);

            if (activeGame != null)
                throw new Exception("Host already has an active game.");
            var activeGameAsPlayer = await _context.GamePlayers
            .Include(gp => gp.game)
            .AnyAsync(gp => gp.userId == hostId && !gp.game.isStarted);
            Console.WriteLine("Halo" + hostId + "halo" + activeGameAsPlayer);
            if (activeGameAsPlayer)
                throw new Exception("User is already in an active game.");

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
            var activeGame = await _context.GamePlayers
            .Include(gp => gp.game)
            .AnyAsync(gp => gp.userId == userId && !gp.game.isStarted);
            Console.WriteLine("Halo" + userId + "halo" + activeGame);
            if (activeGame)
                throw new Exception("User is already in an active game.");



            var game = await _context.Games
            .Include(g => g.GamePlayers)
            .Include(g => g.chest) 
            .FirstOrDefaultAsync(g => g.Id == gameId);

            var user = await _context.Users.FindAsync(userId);
            Console.WriteLine("game" + game);
            Console.WriteLine("user" + user);
            Console.WriteLine("chest" + game.chest);
            if (game == null || user == null)
                throw new Exception("Invalid game or user.");
            
            if (game.isStarted || game.GamePlayers.Count >= game.maxPlayers)
                throw new Exception("Game is full or has already started.");
            Console.WriteLine(_userService.ToString());
            if(user.Balance<game.chest.Price)
            {
                throw new Exception("User does not have enough balance to enter the game");
            }
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
        public async Task<GetLobbyDetailsResponse> GetLobbyDetailsAsync(int gameId)
        {
            var game = await _context.Games
            .Include(g => g.GamePlayers)
            .ThenInclude(gp => gp.user)
            .ThenInclude(u => u.IdentityUser) 
            .FirstOrDefaultAsync(g => g.Id == gameId);
    
            if (game == null)
            {
                throw new Exception("Game not found.");
            }
            var response = new GetLobbyDetailsResponse
            {
                GameId = game.Id,
                HostId = game.hostId,
                IsStarted = game.isStarted
            };
            foreach (var player in game.GamePlayers)
            {
                Console.WriteLine("PIERE" + response.Players);
                Console.WriteLine("PIERE2" + player.userId);
                Console.WriteLine("Piere3" + game.GamePlayers);
                Console.WriteLine("Piere4" + player.user);
                Console.WriteLine("Piere5" + player.user.IdentityUser);
                Console.WriteLine("Piere6" + player.user.IdentityUser.UserName);
                response.Players.Add(new PlayerInfo
                {
                    UserId = player.userId,
                    Username = player.user.IdentityUser.UserName,
                    IsReady = player.isReady
                });
            }
            return response;

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


        public async Task<GameInfo> GetGameInfoAsync(int gameId)
        {
            var game = await _context.Games
                .Include(g => g.GamePlayers)
                .ThenInclude(gp => gp.user)
                .ThenInclude(u => u.IdentityUser)
                .FirstOrDefaultAsync(g => g.Id == gameId);

            if (game == null)
            {
                throw new Exception("Game not found.");
            }

            var gameInfo = new GameInfo
            {
                GameId = game.Id,
                HostId = game.hostId,
                CaseId = game.caseId,
                CurrentPlayers = game.GamePlayers.Count,
                MaxPlayers = game.maxPlayers,
                IsStarted = game.isStarted,
                CreatedAt = Timestamp.FromDateTime(game.createdAt.ToUniversalTime())
            };

            return gameInfo;
        }

        public async Task<int> GetUserGame(int userId)
        {
            
            // Find the active game the user is currently in
            var activeGame = await _context.GamePlayers
                .Include(gp => gp.game) // Include the game details
                .FirstOrDefaultAsync(gp => gp.userId == userId && !gp.game.isStarted);

            // If the user is in an active game, return the gameId
            if (activeGame != null)
            {
                return activeGame.gameId;
            }

            // If the user is not in any active game, return 0
            return 0;
            
      
        }

    }
}
