using Grpc.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GrpcService1.Services.GrpcServices;

public class UserGrpcService : UserGrpc.UserGrpcBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserGrpcService> _logger;
    private readonly UserManager<IdentityUser> _userManager;
    public UserGrpcService(IUserService userService, ILogger<UserGrpcService> logger, UserManager<IdentityUser> userManager)
    {
        _userService = userService;
        _logger = logger;
        _userManager = userManager;
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
        //var user = await _userService.CreateUserAsync(request.UserId);
        /*
        if (user == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "User not found"));
        }

        return new CreateUserResponse
        {
            Id = user.Id,
            IdentityId = user.IdentityId,
            Balance = (double)user.Balance
        };*/
        return null;
    }

    public override async Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
    {
        // Check if the user already exists
        var existingUser = await _userManager.FindByNameAsync(request.Username);
        if (existingUser != null)
        {
            return new RegisterResponse
            {
                Success = false,
                Message = "User already exists."
            };
        }

        // Create a new IdentityUser
        var user = new IdentityUser
        {
            UserName = request.Username,
            Email = request.Email
        };

        // Create the user in the Identity database
        var result = await _userManager.CreateAsync(user, request.Password);
       
        if (result.Succeeded)
        {
            await _userService.CreateUserAsync(user.Id); 
            return new RegisterResponse
            {
                Success = true,
                Message = "User registered successfully."
            };
        }
        else
        {
            return new RegisterResponse
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            };
        }
    }


    public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext context)
    {
        Console.WriteLine("Processing login...");

        var user = await _userManager.FindByNameAsync(request.Username);
        Console.WriteLine($"Attempting to find user with username: {request.Username}");

        if (user == null || !(await _userManager.CheckPasswordAsync(user, request.Password)))
        {
            Console.WriteLine($"Login failed: Invalid username or password for {request.Username}");
            return new LoginResponse
            {
                Success = false,
                Message = "Invalid username or password."
            };
        }

        Console.WriteLine($"User found: {user.UserName}, Id: {user.Id}, Email: {user.Email}");

        var key = Encoding.UTF8.GetBytes("YourSecureKey12345678901234567890123456789012345678");
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email ?? "")
        }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "GrpcServer",
            Audience = "GrpcClients",
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
        };

        try
        {
            Console.WriteLine("Generating JWT token...");

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);

            Console.WriteLine("JWT token successfully generated.");

            return new LoginResponse
            {
                Success = true,
                Token = token,
                Message = "Login successful."
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception during token generation: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }




}