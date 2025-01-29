using Grpc.Core;
using GrpcService1.Models;

namespace GrpcService1.Services.GrpcServices;

public class AdminGrpcService : AdminService.AdminServiceBase
{
    private readonly IChestService _chestService;
    private readonly IItemService _itemService;
    private readonly ILogger<AdminGrpcService> _logger;

    public AdminGrpcService(IChestService chestService, IItemService itemService, ILogger<AdminGrpcService> logger)
    {
        _chestService = chestService;
        _itemService = itemService;
        _logger = logger;
    }

        public override async Task<CreateChestResponse> CreateChest(
            CreateChestRequest request, ServerCallContext context)
        {
            var chest = new Chest
            (
            null,
                request.Name,
                (decimal)request.Price,
                request.PossibleItems.Select(pi => new ChestItem
                (
                    pi.ItemId,
                    (decimal)pi.DropChance
                    
                )).ToList());
            await _chestService.CreateChestAsync(chest);
            return new CreateChestResponse { Chest = new ChestDto { Id = chest.Id, Name = chest.Name, Price = (double)chest.Price } };
        }

        public override async Task<ReadChestResponse> ReadChest(
            ReadChestRequest request, ServerCallContext context)
        {
            var chest = await _chestService.GetChestByIdAsync(request.Id);
            if (chest == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Chest not found"));
            }
            return new ReadChestResponse { Chest = new ChestDto { Id = chest.Id, Name = chest.Name, Price = (double)chest.Price } };
        }

        public override async Task<UpdateChestResponse> UpdateChest(
            UpdateChestRequest request, ServerCallContext context)
        {
            var chest = new Chest
            {
                Id = request.Id,
                Name = request.Name,
                Price = (decimal)request.Price,
                PossibleItems = request.PossibleItems.Select(pi => new ChestItem
                (
                    pi.ItemId,
                    (decimal)pi.DropChance
                    
                )).ToList()
            };
            await _chestService.UpdateChestAsync(chest);
            return new UpdateChestResponse { Chest = new ChestDto { Id = chest.Id, Name = chest.Name, Price = (double)chest.Price } };
        }

        public override async Task<DeleteChestResponse> DeleteChest(
            DeleteChestRequest request, ServerCallContext context)
        {
            var success = await _chestService.DeleteChestAsync(request.Id);
            return new DeleteChestResponse { Success = success };
        }

        public override async Task<ReadAllChestsResponse> ReadAllChests(
            ReadAllChestsRequest request, ServerCallContext context)
        {
            var chests = await _chestService.GetAllChestsAsync();
            return new ReadAllChestsResponse
            {
                Chests =
                {
                    chests.Select(chest => new ChestDto
                    {
                        Id = chest.Id,
                        Name = chest.Name,
                        Price = (double)chest.Price
                    })
                }
            };
        }

    public override async Task<CreateItemResponse> CreateItem(
        CreateItemRequest request, ServerCallContext context)
    {
        try
        {
            var item = new Item(request.Name, (decimal)request.Value, request.ImageUrl);
            await _itemService.CreateItemAsync(item);

            return new CreateItemResponse
            {
                Item = new ItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Value = (double)item.Value,
                    ImageUrl = item.ImageUrl
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating item");
            throw new RpcException(new Status(StatusCode.Internal, "Error creating item"));
        }
    }

    public override async Task<ReadItemResponse> ReadItem(
        ReadItemRequest request, ServerCallContext context)
    {
        var item = await _itemService.GetItemByIdAsync(request.Id);
        if (item == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Item not found"));
        }

        return new ReadItemResponse
        {
            Item = new ItemDto
            {
                Id = item.Id,
                Name = item.Name,
                Value = (double)item.Value,
                ImageUrl = item.ImageUrl
            }
        };
    }

    public override async Task<UpdateItemResponse> UpdateItem(
        UpdateItemRequest request, ServerCallContext context)
    {
        try
        {
            var item = await _itemService.GetItemByIdAsync(request.Id);
            if (item == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Item not found"));
            }

            item.Update(request.Name, (decimal)request.Value, request.ImageUrl);
            await _itemService.UpdateItemAsync(item);

            return new UpdateItemResponse
            {
                Item = new ItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Value = (double)item.Value,
                    ImageUrl = item.ImageUrl
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating item");
            throw new RpcException(new Status(StatusCode.Internal, "Error updating item"));
        }
    }

    public override async Task<DeleteItemResponse> DeleteItem(
        DeleteItemRequest request, ServerCallContext context)
    {
        try
        {
            var success = await _itemService.DeleteItemAsync(request.Id);
            return new DeleteItemResponse { Success = success };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting item");
            throw new RpcException(new Status(StatusCode.Internal, "Error deleting item"));
        }
    }

    public override async Task<GetAllItemsResponse> GetAllItems(
        GetAllItemsRequest request, ServerCallContext context)
    {
        var items = await _itemService.GetAllItemsAsync();
        return new GetAllItemsResponse
        {
            Items =
            {
                items.Select(item => new ItemDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Value = (double)item.Value,
                    ImageUrl = item.ImageUrl
                })
            }
        };
    }
}
