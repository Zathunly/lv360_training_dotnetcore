using lv360_training.Api.Dtos.Auth;
using lv360_training.Domain.Interfaces.Repositories.Core;
using lv360_training.Domain.Interfaces.Repositories.Auth;
using lv360_training.Domain.Interfaces.Security;
using lv360_training.Domain;
using System.Security.Claims;

namespace lv360_training.Application.Handlers;

public class AuthHandler
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IUserRoleRepository _userRoles;
    private readonly ISessionRepository _sessions;
    private readonly IUnitOfWork _uow;
    private readonly IPasswordService _hasher;

    public AuthHandler(
        IUserRepository users,
        IRoleRepository roles,
        IUserRoleRepository userRoles,
        ISessionRepository sessions,
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
        var sessionIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value;
        if (sessionIdClaim == null || !Guid.TryParse(sessionIdClaim, out var sessionId))
            return false;

        var session = await _sessions.GetByIdAsync(sessionId);

        return session is { ExpiresAt: var exp } && exp > DateTime.UtcNow;
    }

    public async Task<Session> CreateSessionAsync(User user)
    {
        var session = new Session
        {
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            CreatedAt = DateTime.UtcNow
        };

        await _sessions.AddAsync(session);
        await _uow.SaveChangesAsync();

        return session;
    }

    public async Task LogoutUserAsync(ClaimsPrincipal principal)
    {
        var sessionIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value;
        if (sessionIdClaim == null) return;

        if (Guid.TryParse(sessionIdClaim, out var sessionId))
        {
            await _sessions.DeleteAsync(sessionId);
            await _uow.SaveChangesAsync();
        }
    }
}
