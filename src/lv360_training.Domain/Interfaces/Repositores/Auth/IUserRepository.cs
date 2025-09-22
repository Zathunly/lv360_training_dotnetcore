using lv360_training.Domain.Entities;

namespace lv360_training.Domain.Interfaces.Repositories.Auth;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task AddAsync(User user);
    Task<bool> HasRoleAsync(int userId, string roleName);
}
