using Grpc.Core;
using GrpcService1.Models;

namespace GrpcService1.Services.GrpcServices;

public class AdminGrpcService : AdminService.AdminServiceBase
{
    private readonly IChestService _chestService;
    private readonly ILogger<AdminGrpcService> _logger;

    public AdminGrpcService(IChestService chestService, ILogger<AdminGrpcService> logger)
    {
        _chestService = chestService;
        _logger = logger;
    }

    public override async Task<CreateChestResponse> CreateChest(
        CreateChestRequest request, ServerCallContext context)
    {
        try
        {
            var chest = new Chest(request.Name, (decimal)request.Price);

            foreach (var item in request.PossibleItems)
            {
                chest.AddPossibleItem(item.ItemId, (decimal)item.DropChance);
            }

            await _chestService.CreateChestAsync(chest);

            return new CreateChestResponse
            {
                Chest = new ChestDto
                {
                    Id = chest.Id,
                    Name = chest.Name,
                    Price = (double)chest.Price,
                    PossibleItems =
                    {
                        chest.PossibleItems.Select(pi => new ChestItemDto
                        {
                            Item = new ItemDto
                            {
                                Id = pi.Item.Id,
                                Name = pi.Item.Name,
                                Value = (double)pi.Item.Value,
                                ImageUrl = pi.Item.ImageUrl
                            },
                            DropChance = (double)pi.DropChance
                        })
                    }
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating chest");
            throw new RpcException(new Status(StatusCode.Internal, "Error creating chest"));
        }
    }

    public override async Task<ReadChestResponse> ReadChest(
        ReadChestRequest request, ServerCallContext context)
    {
        var chest = await _chestService.GetChestByIdAsync(request.Id);
        if (chest == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Chest not found"));
        }

        return new ReadChestResponse
        {
            Chest = new ChestDto
            {
                Id = chest.Id,
                Name = chest.Name,
                Price = (double)chest.Price,
                PossibleItems =
                {
                    chest.PossibleItems.Select(pi => new ChestItemDto
                    {
                        Item = new ItemDto
                        {
                            Id = pi.Item.Id,
                            Name = pi.Item.Name,
                            Value = (double)pi.Item.Value,
                            ImageUrl = pi.Item.ImageUrl
                        },
                        DropChance = (double)pi.DropChance
                    }).ToList()
                }

            }
        };
    }

    public override async Task<UpdateChestResponse> UpdateChest(
        UpdateChestRequest request, ServerCallContext context)
    {
        try
        {
            var chest = await _chestService.GetChestByIdAsync(request.Id);
            if (chest == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Chest not found"));
            }

            chest.Update(request.Name, (decimal)request.Price);
            chest.ClearPossibleItems();

            foreach (var item in request.PossibleItems)
            {
                chest.AddPossibleItem(item.ItemId, (decimal)item.DropChance);
            }

            await _chestService.UpdateChestAsync(chest);

            return new UpdateChestResponse
            {
                Chest = new ChestDto
                {
                    Id = chest.Id,
                    Name = chest.Name,
                    Price = (double)chest.Price,
                    PossibleItems =
                    {
                        chest.PossibleItems.Select(pi => new ChestItemDto
                        {
                            Item = new ItemDto
                            {
                                Id = pi.Item.Id,
                                Name = pi.Item.Name,
                                Value = (double)pi.Item.Value,
                                ImageUrl = pi.Item.ImageUrl
                            },
                            DropChance = (double)pi.DropChance
                        })
                    }

                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating chest");
            throw new RpcException(new Status(StatusCode.Internal, "Error updating chest"));
        }
    }

    public override async Task<DeleteChestResponse> DeleteChest(
        DeleteChestRequest request, ServerCallContext context)
    {
        try
        {
            var success = await _chestService.DeleteChestAsync(request.Id);
            return new DeleteChestResponse { Success = success };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting chest");
            throw new RpcException(new Status(StatusCode.Internal, "Error deleting chest"));
        }
    }
}