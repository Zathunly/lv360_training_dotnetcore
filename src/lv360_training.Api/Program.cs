using lv360_training.Application.Handlers;
using lv360_training.Application.Interfaces.Repositories.Auth;
using lv360_training.Application.Interfaces.Repositories.Core;
using lv360_training.Application.Interfaces.Security;
using lv360_training.Infrastructure.Db;
using lv360_training.Infrastructure.Repositories.Core;
using lv360_training.Infrastructure.Security;
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


// --- Register Infrastructure services ---
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();   
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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
