using lv360_training.Domain;

namespace lv360_training.Application.Interfaces.Repositories.Auth;

public interface IUserRoleRepository
{
    Task AddAsync(UserRole userRole);
    Task<UserRole?> GetAsync(int userId, int roleId);
    Task<bool> ExistsAsync(int userId, int roleId);
    Task<List<Role>> GetRolesForUserAsync(int userId);
}
