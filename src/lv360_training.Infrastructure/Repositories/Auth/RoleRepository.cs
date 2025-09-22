using lv360_training.Domain.Interfaces.Repositories.Auth;
using lv360_training.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using lv360_training.Infrastructure.Persistence;

namespace lv360_training.Infrastructure.Repositories.Auth;

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
