using lv360_training.Domain;

namespace lv360_training.Application.Interfaces.Repositories.Auth;

public interface IPermissionRepository
{
    Task<Permission?> GetByNameAsync(string name);
    Task AddToRoleAsync(RolePermission rolePermission);
}
