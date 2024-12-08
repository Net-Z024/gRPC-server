using Grpc.Core;
using GrpcService1;
using GrpcService1.Services;
using YourNamespace.Protos;

public class UserGrpcService : UserGrpc.UserGrpcBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserGrpcService> _logger;

    public UserGrpcService(IUserService userService, ILogger<UserGrpcService> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public override async Task<GetUserResponse> GetUser(
        GetUserRequest request, ServerCallContext context)
    {
        var user = await _userService.GetByIdAsync(request.UserId);
        
        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        return new GetUserResponse
        {
            Id = user.Id,
            IdentityId = user.IdentityId,
            Balance = (double)user.Balance
        };
    }

    public override async Task<CreateUserResponse> CreateUser(
        CreateUserRequest request, ServerCallContext context)
    {
        try
        {
            var user = await _userService.CreateUserAsync(request.IdentityId);

            return new CreateUserResponse
            {
                Id = user.Id,
                IdentityId = user.IdentityId,
                Balance = (double)user.Balance
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            throw new RpcException(new Status(StatusCode.Internal, "Error creating user"));
        }
    }

    public override async Task<BalanceResponse> AddBalance(
        AddBalanceRequest request, ServerCallContext context)
    {
        try
        {
            var success = await _userService.AddBalanceAsync(
                request.UserId, (decimal)request.Amount);

            if (!success)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
            }

            var user = await _userService.GetByIdAsync(request.UserId);
            return new BalanceResponse
            {
                Success = true,
                NewBalance = (double)user.Balance
            };
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
    }

    public override async Task<BalanceResponse> SpendBalance(
        SpendBalanceRequest request, ServerCallContext context)
    {
        try
        {
            var success = await _userService.SpendBalanceAsync(
                request.UserId, (decimal)request.Amount);

            if (!success)
            {
                return new BalanceResponse
                {
                    Success = false,
                    NewBalance = (double)(await _userService.GetByIdAsync(request.UserId))?.Balance
                };
            }

            var user = await _userService.GetByIdAsync(request.UserId);
            return new BalanceResponse
            {
                Success = true,
                NewBalance = (double)user.Balance
            };
        }
        catch (ArgumentException ex)
        {
            throw new RpcException(new Status(StatusCode.InvalidArgument, ex.Message));
        }
    }
}