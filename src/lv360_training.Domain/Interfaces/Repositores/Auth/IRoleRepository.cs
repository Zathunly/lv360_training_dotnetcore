using lv360_training.Domain.Entities;

namespace lv360_training.Domain.Interfaces.Repositories.Auth;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string roleName);
    Task RemoveUserRoleAsync(int userId);
    Task AddAsync(Role role);
    Task AddUserRoleAsync(UserRole userRole);
}
