using lv360_training.Application.Interfaces.Repositories.Auth;
using lv360_training.Domain;
using Microsoft.EntityFrameworkCore;

namespace lv360_training.Infrastructure.Db;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetByNameAsync(string roleName) =>
        await _context.Roles.FirstOrDefaultAsync(r => r.Name == roleName);

    public async Task AddAsync(Role role) =>
        await _context.Roles.AddAsync(role);

    public async Task AddUserRoleAsync(UserRole userRole) =>
        await _context.UserRoles.AddAsync(userRole);
}
