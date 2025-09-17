using lv360_training.Infrastructure.Db;
using lv360_training.Application.Interfaces;
using lv360_training.Domain;
using lv360_training.Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace lv360_training.Seeder;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length != 3 || args[0] != "--admin")
        {
            Console.WriteLine("Usage: Seeder --admin <username> <password>");
            return;
        }

        var username = args[1];
        var password = args[2];

        // --- Setup DI ---
        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                "server=db;port=3306;database=lv360_training;user=root;password=secret;",
                new MySqlServerVersion(new Version(8, 0, 33))
            )
        );

        services.AddScoped<IDbService, DbService>();
        services.AddScoped<IAuthService, AuthService>();

        var provider = services.BuildServiceProvider();

        var db = provider.GetRequiredService<IDbService>();
        var auth = provider.GetRequiredService<IAuthService>();

        // --- Check if user exists ---
        var existing = await db.GetUserByUsernameAsync(username);
        if (existing != null)
        {
            Console.WriteLine($"User '{username}' already exists.");
            return;
        }

        // --- Create admin user ---
        var user = new User
        {
            Username = username,
            Password = auth.HashPassword(password)
        };
        await db.AddUserAsync(user);
        await db.SaveChangesAsync();

        // --- Assign Admin role ---
        var adminRole = await db.GetRoleByNameAsync("Admin");
        if (adminRole != null)
        {
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = adminRole.Id
            };
            await db.AddUserRoleAsync(userRole);
            await db.SaveChangesAsync();
        }

        Console.WriteLine($"Admin user '{username}' created successfully.");
    }
}
