using lv360_training.Application.Handlers;
using lv360_training.Application;
using lv360_training.Infrastructure;
using lv360_training.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// --- Add DbContext ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 33))
    )
);

// Add authentication with cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/api/auth/login";     
        options.LogoutPath = "/api/auth/logout";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; 
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

// --- Add session ---
builder.Services.AddDistributedMemoryCache(); 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// --- Register Services per Layer ---
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();

// --- Add Controllers ---
builder.Services.AddControllers();

// --- Build App ---
var app = builder.Build();

// --- Middleware ---
app.UseRouting();
app.UseAuthentication();  
app.UseAuthorization();
app.UseSession();
app.MapControllers();   

// --- Run App ---
app.Run();
