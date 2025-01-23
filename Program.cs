using GrpcService1.Data;
using GrpcService1.Services.GrpcServices;
using GrpcService1.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
var jwtKey = builder.Configuration["Jwt:Key"] ?? "YourSecureKey12345678901234567890123456789012345678"; // tu ustawic trzeba konfiguracje potem
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "GrpcServer";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "GrpcClients";
// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();




// Register all services BEFORE building the app
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Register your services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChestService, ChestService>();
builder.Services.AddScoped<IItemService, ItemService>();

// Build the app AFTER registering all services
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
// Configure the HTTP request pipeline.
app.MapGrpcService<UserGrpcService>();
app.MapGrpcService<ChestGrpcService>();
app.MapGrpcService<ItemGrpcService>();
app.MapGrpcService<GameGrpcService>();
app.MapGrpcReflectionService();

app.MapGet("/", () =>
    "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2099682");

app.Run();