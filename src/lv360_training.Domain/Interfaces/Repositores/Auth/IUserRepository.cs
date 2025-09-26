using lv360_training.Domain.Entities;

namespace lv360_training.Domain.Interfaces.Repositories.Auth;

public interface IUserRepository
{
    Task AddAsync(User user);

    Task<User?> GetByIdAsync(int userId); 
    Task<User?> GetByUsernameAsync(string username);
    Task<Role?> GetRoleByUserIdAsync(int userId);
    Task<bool> HasRoleAsync(int userId, string roleName);
}
