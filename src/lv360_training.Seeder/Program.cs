using lv360_training.Infrastructure.Persistence;
using lv360_training.Domain.Entities;
using lv360_training.Domain.Interfaces.Security;
using lv360_training.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace lv360_training.Seeder;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  Seeder --role");
            Console.WriteLine("  Seeder --admin <username> <password>");
            return;
        }

        // --- Setup DI ---
        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(
                "server=db;port=3306;database=lv360_training;user=root;password=secret;",
                new MySqlServerVersion(new Version(8, 0, 33))
            )
        );

        services.AddScoped<IPasswordService, PasswordService>();

        var provider = services.BuildServiceProvider();
        var context = provider.GetRequiredService<AppDbContext>();
        var auth = provider.GetRequiredService<IPasswordService>();

        // --- Handle --role ---
        if (args[0] == "--role")
        {
            await EnsureRoleExists(context, "Admin");
            await EnsureRoleExists(context, "User");
            Console.WriteLine("Roles 'Admin' and 'User' ensured.");
            return;
        }

        // --- Handle --admin ---
        if (args[0] == "--admin")
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: Seeder --admin <username> <password>");
                return;
            }

            var username = args[1];
            var password = args[2];

            await EnsureRoleExists(context, "Admin");
            await EnsureRoleExists(context, "User");

            // Check if user exists
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (existingUser != null)
            {
                Console.WriteLine($"⚠️ User '{username}' already exists.");
                return;
            }

            // Create admin user
            var user = new User
            {
                Username = username,
                Password = auth.HashPassword(password)
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            if (adminRole != null)
            {
                context.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = adminRole.Id
                });
                await context.SaveChangesAsync();
            }

            Console.WriteLine($"Admin user '{username}' created successfully.");
        }
    }

    private static async Task EnsureRoleExists(AppDbContext context, string roleName)
    {
        var role = await context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
        if (role == null)
        {
            role = new Role { Name = roleName };
            context.Roles.Add(role);
            await context.SaveChangesAsync();
        }
    }
}
