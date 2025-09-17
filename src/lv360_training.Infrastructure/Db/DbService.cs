using lv360_training.Application.Interfaces;
using lv360_training.Domain;
using Microsoft.EntityFrameworkCore;

namespace lv360_training.Infrastructure.Db;

public class DbService : IDbService
{
    private readonly AppDbContext _context;

    public DbService(AppDbContext context)
    {
        _context = context;
    }

    // --- Users ---
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await Task.CompletedTask;
    }

    public async Task<bool> UserHasRoleAsync(int userId, string roleName)
    {
        return await _context.UserRoles
            .Include(ur => ur.Role)
            .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == roleName);
    }

    // --- Roles ---
    public async Task<Role?> GetRoleByNameAsync(string roleName)
    {
        return await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == roleName);
    }

    public async Task AddUserRoleAsync(UserRole userRole)
    {
        _context.UserRoles.Add(userRole);
        await Task.CompletedTask;
    }

    // --- Permissions ---
    public async Task<Permission?> GetPermissionByNameAsync(string permissionName)
    {
        return await _context.Permissions
            .FirstOrDefaultAsync(p => p.Name == permissionName);
    }

    public async Task AddRolePermissionAsync(RolePermission rolePermission)
    {
        _context.Set<RolePermission>().Add(rolePermission);
        await Task.CompletedTask;
    }

    public async Task AddSession(Session session)
    {
        _context.Sessions.Add(session);
        await Task.CompletedTask;
    }

    // --- Get session by ID ---
    public async Task<Session?> GetSessionByIdAsync(Guid sessionId)
    {
        return await _context.Sessions
            .Include(s => s.User) // optional: include user info
            .FirstOrDefaultAsync(s => s.Id == sessionId);
    }

    // --- Delete session ---
    public async Task DeleteSessionAsync(Guid sessionId)
    {
        var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        if (session != null)
        {
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync(); 
        }
    }

    // --- Save Changes ---
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
