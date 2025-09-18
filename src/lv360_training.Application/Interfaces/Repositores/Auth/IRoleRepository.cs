using lv360_training.Domain;

namespace lv360_training.Application.Interfaces.Repositories.Auth;

public interface IRoleRepository
{
    Task<Role?> GetByNameAsync(string roleName);
    Task AddAsync(Role role);
    Task AddUserRoleAsync(UserRole userRole);
}
