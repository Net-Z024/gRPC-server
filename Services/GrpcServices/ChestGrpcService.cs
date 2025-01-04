using Grpc.Core;

namespace GrpcService1.Services.GrpcServices;

public class ChestGrpcService : ChestGrpc.ChestGrpcBase
{
    private readonly IChestService _chestService;
    private readonly IUserService _userService;
    private readonly ILogger<ChestGrpcService> _logger;

    public ChestGrpcService(IChestService chestService, IUserService userService, ILogger<ChestGrpcService> logger)
    {
        _chestService = chestService;
        _userService = userService;
        _logger = logger;
    }

    public override async Task<GetChestsResponse> GetChests(
        GetChestsRequest request, ServerCallContext context)
    {
        var chests = await _chestService.GetAllChestsAsync();
        
        var response = new GetChestsResponse();
        response.Chests.AddRange(chests.Select(c => new ChestDto
        {
            Id = c.Id,
            Name = c.Name,
            Price = (double)c.Price,
            PossibleItems = 
            {
                c.PossibleItems.Select(cp => new ChestItemDto
                {
                    Item = new ItemDto
                    {
                        Id = cp.Item.Id,
                        Name = cp.Item.Name,
                        Value = (double)cp.Item.Value,
                        ImageUrl = cp.Item.ImageUrl

                    },
                    DropChance = (double)cp.DropChance
                })
            }
        }));

        return response;
    }

    public override async Task<OpenChestResponse> OpenChest(
        OpenChestRequest request, ServerCallContext context)
    {
        try
        {
            var item = await _chestService.OpenChestAsync(request.UserId, request.ChestId);
            var user = await _userService.GetByIdAsync(request.UserId);

            return new OpenChestResponse
            {
                Success = true,
                ReceivedItem = new ItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Value = (double)item.Value,
                    ImageUrl = item.ImageUrl
                },
                NewBalance = (double)user.Balance
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening chest");
            return new OpenChestResponse { Success = false };
        }
    }
}