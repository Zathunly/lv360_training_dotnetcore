using lv360_training.Domain.Interfaces.Repositories.Auth;
using lv360_training.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using lv360_training.Infrastructure.Persistence;

namespace lv360_training.Infrastructure.Repositories.Auth;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == username);

    public async Task AddAsync(User user) =>
        await _context.Users.AddAsync(user);

    public async Task<bool> HasRoleAsync(int userId, string roleName) =>
        await _context.UserRoles
            .Include(ur => ur.Role)
            .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == roleName);
}
