﻿using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcService1.Models;
using GrpcService1.Services;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace GrpcService1.Services.GrpcServices
{
    public class GameGrpcService : GameGrpc.GameGrpcBase
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GameGrpcService> _logger;

        public GameGrpcService(IGameService gameService, ILogger<GameGrpcService> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }
        public override async Task<getGameInfoResponse> GetGameInfo(getGameInfoRequest request, ServerCallContext context)
        {
            try
            {
                var game = await _gameService.GetGameInfoAsync(request.GameId);

                return new getGameInfoResponse
                {
                    GameInfo=game
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error creating game");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }
        public override async Task<CreateGameResponse> CreateGame(CreateGameRequest request, ServerCallContext context)
        {
            try
            {
                var game = await _gameService.CreateGameAsync(request.HostId, request.CaseId, request.MaxPlayers);

                return new CreateGameResponse
                {
                    GameId = game.Id,
                    CreatedAt = Timestamp.FromDateTime(game.createdAt.ToUniversalTime()),
                    Message = "Game created successfully!"
                };
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error creating game");
                return new CreateGameResponse { Message = ex.Message };
            }
        }

        public override async Task<JoinGameResponse> JoinGame(JoinGameRequest request, ServerCallContext context)
        {
            try
            {
                await _gameService.JoinGameAsync(request.GameId, request.UserId);
                return new JoinGameResponse { Message = "Joined game successfully!", Success = true };
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error joining game");
                return new JoinGameResponse { Message = ex.Message, Success = false };
            }
        }

        public override async Task<MarkReadyResponse> MarkReady(MarkReadyRequest request, ServerCallContext context)
        {
            try
            {
                await _gameService.MarkReadyAsync(request.GameId, request.UserId);
                return new MarkReadyResponse { Message = "Player marked as ready.", Success = true };
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error marking ready");
                return new MarkReadyResponse { Message = ex.Message, Success = false };
            }
        }

        public override async Task<StartGameResponse> StartGame(StartGameRequest request, ServerCallContext context)
        {
            try
            {
                var response = await _gameService.StartGameAsync(request.GameId);
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error starting game");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }
        public override async Task<GetLobbyDetailsResponse> GetLobbyDetails(GetLobbyDetailsRequest request, ServerCallContext context)
        {


            try
            {
                var response = await _gameService.GetLobbyDetailsAsync(request.GameId);
                 return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching lobby info");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }


           
        }

        public override async Task<GetOpenGamesResponse> GetOpenGames(GetOpenGamesRequest request, ServerCallContext context)
        {
            try
            {
                var openGames = await _gameService.GetOpenGamesAsync();

                var response = new GetOpenGamesResponse();
                foreach (var game in openGames)
                {
                    response.Games.Add(new GameInfo
                    {
                        
                        GameId = game.Id,
                        HostId = game.hostId,
                        CaseId = game.caseId,
                        CurrentPlayers = game.GamePlayers.Count,
                        MaxPlayers = game.maxPlayers,
                        IsStarted = game.isStarted,
                        CreatedAt = Timestamp.FromDateTime(game.createdAt.ToUniversalTime())
                    });
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching open games");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }


        public override async Task<getUserGameResponse> GetUserGame(getUserGameRequest request, ServerCallContext context)
        {
            try
            {
                // Find the active game the user is currently in
                var userGame = await _gameService.GetUserGame(request.UserId);

                return new getUserGameResponse
                {
                    GameId = userGame
                };

             

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user's active game");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

    }
}
