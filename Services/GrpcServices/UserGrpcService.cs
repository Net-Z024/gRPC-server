using Grpc.Core;

namespace GrpcService1.Services.GrpcServices;

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
}