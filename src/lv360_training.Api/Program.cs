using lv360_training.Application.Handlers;
using lv360_training.Application.Interfaces;
using lv360_training.Infrastructure.Db;
using lv360_training.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// --- Add DbContext ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 33)) // MySQL version
    )
);


// --- Register Infrastructure services ---
builder.Services.AddScoped<IDbService, DbService>();

// --- Register Auth services ---
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IAuthService, AuthService>();

// --- Register Application handlers ---
builder.Services.AddScoped<AuthHandler>();

// --- Add Controllers ---
builder.Services.AddControllers();

// --- Build App ---
var app = builder.Build();

// --- Middleware ---
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// --- Run App ---
app.Run();
