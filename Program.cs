using GrpcService1.Data;
using GrpcService1.Services;
using GrpcService1.Services.GrpcServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Register all services BEFORE building the app
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

// Register your services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IChestService, ChestService>();
builder.Services.AddScoped<IItemService, ItemService>();

// Build the app AFTER registering all services
var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<UserGrpcService>();
app.MapGrpcService<ChestGrpcService>();
app.MapGrpcService<ItemGrpcService>();
app.MapGrpcReflectionService();

app.MapGet("/", () =>
    "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2099682");

app.Run();