using lv360_training.Domain.Interfaces.Repositories.Auth;
using lv360_training.Domain;
using Microsoft.EntityFrameworkCore;

using lv360_training.Infrastructure.Persistence;

namespace lv360_training.Infrastructure.Repositories.Auth;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly AppDbContext _context;

    public UserRoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(UserRole userRole)
    {
        await _context.UserRoles.AddAsync(userRole);
        // save is handled by IUnitOfWork
    }

    public async Task<UserRole?> GetAsync(int userId, int roleId)
    {
        return await _context.UserRoles
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }

    public async Task<bool> ExistsAsync(int userId, int roleId)
    {
        return await _context.UserRoles
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
    }

    public async Task<List<Role>> GetRolesForUserAsync(int userId)
    {
        return await _context.UserRoles
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role) 
            .Select(ur => ur.Role!)
            .ToListAsync();
    }
}
