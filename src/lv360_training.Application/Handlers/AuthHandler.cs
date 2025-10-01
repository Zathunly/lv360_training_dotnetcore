using lv360_training.Domain.Dtos.Auth.Request;
using lv360_training.Domain.Interfaces.Repositories.Core;
using lv360_training.Domain.Interfaces.Repositories.Auth;
using lv360_training.Domain.Interfaces.Security;
using lv360_training.Domain.Entities;
using System.Security.Claims;
using lv360_training.Domain.Services.Redis;

namespace lv360_training.Application.Handlers;

public class AuthHandler
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IUserRoleRepository _userRoles;
    private readonly IRedisSessionService _sessions;
    private readonly IUnitOfWork _uow;
    private readonly IPasswordService _hasher;

    public AuthHandler(
        IUserRepository users,
        IRoleRepository roles,
        IUserRoleRepository userRoles,
        IRedisSessionService sessions,
        IUnitOfWork uow,
        IPasswordService hasher)
    {
        _users = users;
        _roles = roles;
        _userRoles = userRoles;
        _sessions = sessions;
        _uow = uow;
        _hasher = hasher;
    }

    public async Task<int> Register(RegisterRequest request, string roleName)
    {
        // Ensure username is unique
        if (await _users.GetByUsernameAsync(request.Username) != null)
            throw new Exception("Username already exists");

        // Create user
        var user = new User
        {
            Username = request.Username,
            Password = _hasher.HashPassword(request.Password)
        };

        await _users.AddAsync(user);
        await _uow.SaveChangesAsync();

        // Assign role
        var role = await _roles.GetByNameAsync(roleName);
        if (role == null)
            throw new Exception($"Role '{roleName}' not found");

        await _userRoles.AddAsync(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });
        await _uow.SaveChangesAsync();

        return user.Id;
    }

    public async Task<User?> ValidateUserAsync(LoginRequest request)
    {
        var user = await _users.GetByUsernameAsync(request.Username);

        if (user == null)
            return null;

        return _hasher.VerifyPassword(request.Password, user.Password)
            ? user
            : null;
    }

    public async Task<bool> ValidateSessionAsync(ClaimsPrincipal principal)
    {
        var sessionId = principal.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value;
        if (string.IsNullOrEmpty(sessionId)) return false;

        var session = await _sessions.GetSessionAsync(sessionId);
        if (session == null || session.ExpiresAt <= DateTime.UtcNow)
            return false;

        return true;
    }

    public async Task<User?> GetUserFromClaimsAsync(ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            return null;

        return await _users.GetByIdAsync(userId);
    }

    public async Task<(string SessionId, RedisSession Session)> CreateSessionAsync(User user)
    {
        var sessionId = Guid.NewGuid().ToString();

        var redisSession = new RedisSession
        {
            UserId = user.Id,
            Username = user.Username,
            Roles = user.UserRoles.Select(r => r.Role.Name).ToList(),
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        // Redis handles single-session-per-user internally
        await _sessions.CreateSessionAsync(sessionId, redisSession, TimeSpan.FromHours(2));

        return (sessionId, redisSession);
    }


    public async Task LogoutUserAsync(ClaimsPrincipal principal)
    {
        var sessionId = principal.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value;
        if (!string.IsNullOrEmpty(sessionId))
        {
            await _sessions.DeleteSessionAsync(sessionId);
        }
    }

}
