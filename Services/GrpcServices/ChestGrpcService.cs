using Grpc.Core;
using GrpcService1;
using GrpcService1.Services;

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
            PossibleProducts = 
            {
                c.PossibleProducts.Select(cp => new ChestProductDto
                {
                    Product = new ProductDto
                    {
                        Id = cp.Product.Id,
                        Name = cp.Product.Name,
                        Value = (double)cp.Product.Value
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
            var product = await _chestService.OpenChestAsync(request.UserId, request.ChestId);
            var user = await _userService.GetByIdAsync(request.UserId);

            return new OpenChestResponse
            {
                Success = true,
                ReceivedProduct = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Value = (double)product.Value
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