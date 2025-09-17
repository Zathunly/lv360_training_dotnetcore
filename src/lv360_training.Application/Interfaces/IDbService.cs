using lv360_training.Domain;

namespace lv360_training.Application.Interfaces;

public interface IDbService
{
    // --- Users ---
    Task<User?> GetUserByUsernameAsync(string username);
    Task AddUserAsync(User user);

    // --- Roles ---
    Task<Role?> GetRoleByNameAsync(string roleName);
    Task AddUserRoleAsync(UserRole userRole);

    // --- Sessions ---
    Task AddSession(Session session);
    Task<Session?> GetSessionByIdAsync(Guid sessionId);
    Task DeleteSessionAsync(Guid sessionId);
    
    // --- Permissions ---
    Task<Permission?> GetPermissionByNameAsync(string permissionName);
    Task AddRolePermissionAsync(RolePermission rolePermission);

    // --- Helpers ---
    Task<bool> UserHasRoleAsync(int userId, string roleName);

    // --- Commit ---
    Task<int> SaveChangesAsync();
}
