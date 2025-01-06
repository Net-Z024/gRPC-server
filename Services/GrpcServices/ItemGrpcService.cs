using Grpc.Core;

namespace GrpcService1.Services.GrpcServices;

public class ItemGrpcService : ItemGrpc.ItemGrpcBase
{
    private readonly IItemService _ItemService;
    private readonly ILogger<ItemGrpcService> _logger;

    public ItemGrpcService(IItemService ItemService, ILogger<ItemGrpcService> logger)
    {
        _ItemService = ItemService;
        _logger = logger;
    }

    public override async Task<GetUserItemsResponse> GetUserItems(
        GetUserItemsRequest request, ServerCallContext context)
    {
        var Items = await _ItemService.GetUserItemsAsync(request.UserId);
        
        var response = new GetUserItemsResponse();
        response.Items.AddRange(Items.Select(up => new UserItemDto
        {
            Id = up.Id,
            Item = new ItemDto
            {
                Id = up.Item.Id,
                Name = up.Item.Name,
                Value = (double)up.Item.Value,
                ImageUrl = up.Item.ImageUrl
            }
        }));

        return response;
    }

    public override async Task<GetChestItemsResponse> GetChestItems(
     GetChestItemsRequest request, ServerCallContext context)
    {
        // Fetch ChestItems for the given ChestId
        var chestItems = await _ItemService.GetChestItemsAsync(request.ChestId);

        // Prepare the response
        var response = new GetChestItemsResponse();

        // Map ChestItems to ItemDto and add to the response
        response.Items.AddRange(chestItems.Select(ci => new ItemDto
        {
            Id = ci.Item.Id,
            Name = ci.Item.Name,
            Value = (double)ci.Item.Value,
            ImageUrl = ci.Item.ImageUrl
        }));

        return response;
    }


    public override async Task<SellItemResponse> SellItem(
        SellItemRequest request, ServerCallContext context)
    {
        var (success, newBalance) = await _ItemService.SellItemAsync(
            request.UserId, request.UserItemId);

        return new SellItemResponse
        {
            Success = success,
            NewBalance = (double)newBalance
        };
    }
}