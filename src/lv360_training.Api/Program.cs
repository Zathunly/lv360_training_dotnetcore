using lv360_training.Application.Handlers;
using lv360_training.Application.Interfaces;
using lv360_training.Infrastructure.Db;
using lv360_training.Infrastructure.Auth;
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
        options.LoginPath = "/api/auth/login";     // redirect path if not authenticated
        options.LogoutPath = "/api/auth/logout";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // HTTPS only
        options.ExpireTimeSpan = TimeSpan.FromHours(1);
        options.SlidingExpiration = true;
    });

// --- Add session ---
builder.Services.AddDistributedMemoryCache(); // <-- required for session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// --- Register Infrastructure services ---
builder.Services.AddScoped<IDbService, DbService>();

// --- Register Auth services ---
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IAuthService, AuthService>();

// --- Register Application handlers ---
builder.Services.AddScoped<AuthHandler>();
builder.Services.AddScoped<AdminHandler>();

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
