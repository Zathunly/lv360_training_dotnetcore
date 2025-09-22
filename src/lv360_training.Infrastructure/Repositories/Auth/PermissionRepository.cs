using lv360_training.Domain.Interfaces.Repositories.Auth;
using lv360_training.Domain;
using Microsoft.EntityFrameworkCore;
using lv360_training.Infrastructure.Persistence;

namespace lv360_training.Infrastructure.Repositories.Auth;

public class PermissionRepository : IPermissionRepository
{
    private readonly AppDbContext _context;

    public PermissionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Permission?> GetByNameAsync(string name) =>
        await _context.Permissions.FirstOrDefaultAsync(p => p.Name == name);

    public async Task AddToRoleAsync(RolePermission rolePermission) =>
        await _context.Set<RolePermission>().AddAsync(rolePermission);
}