using lv360_training.Domain.Entities;

namespace lv360_training.Domain.Interfaces.Repositories.Auth;

public interface IPermissionRepository
{
    Task<Permission?> GetByNameAsync(string name);
    Task AddToRoleAsync(RolePermission rolePermission);
}
