using lv360_training.Api.Dtos.Auth;
using lv360_training.Application.Interfaces;
using lv360_training.Domain;
using System.Security.Claims; 

namespace lv360_training.Application.Handlers;

public class AuthHandler
{
    private readonly IDbService _db;
    private readonly IAuthService _auth;

    public AuthHandler(IDbService db, IAuthService auth)
    {
        _db = db;
        _auth = auth;
    }

    public async Task<int> Register(RegisterRequest request, string roleName)
    {
        var existing = await _db.GetUserByUsernameAsync(request.Username);
        if (existing != null)
            throw new Exception("Username already exists");

        var user = new User
        {
            Username = request.Username,
            Password = _auth.HashPassword(request.Password)
        };

        await _db.AddUserAsync(user);
        await _db.SaveChangesAsync();

        var role = await _db.GetRoleByNameAsync(roleName);
        if (role != null)
        {
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            await _db.AddUserRoleAsync(userRole);
            await _db.SaveChangesAsync();
        }

        return user.Id;
    }

    public async Task<User?> ValidateUserAsync(LoginRequest request)
    {
        // Include roles when fetching the user
        var user = await _db.GetUserByUsernameAsync(request.Username);

        if (user == null || !_auth.VerifyPassword(request.Password, user.Password))
            return null;

        // Eagerly load roles if not already loaded
        if (user.UserRoles == null || !user.UserRoles.Any())
        {
            var dbUser = await _db.GetUserByUsernameAsync(request.Username);
            user.UserRoles = dbUser?.UserRoles ?? new List<UserRole>();
        }

        return user;
    }

    public async Task<bool> ValidateSessionAsync(ClaimsPrincipal user)
    {
        var sessionIdClaim = user.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value;
        if (sessionIdClaim == null || !Guid.TryParse(sessionIdClaim, out var sessionId))
            return false;

        var session = await _db.GetSessionByIdAsync(sessionId);
        return session != null && session.ExpiresAt > DateTime.UtcNow;
    }

    public async Task<Session> CreateSessionAsync(User user)
    {
        var session = new Session
        {
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            CreatedAt = DateTime.UtcNow
        };

        await _db.AddSession(session);
        await _db.SaveChangesAsync();

        return session;
    }

    public async Task LogoutUserAsync(ClaimsPrincipal user)
    {
        var sessionIdClaim = user.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value;
        if (sessionIdClaim == null) return;

        if (Guid.TryParse(sessionIdClaim, out var sessionId))
        {
            await _db.DeleteSessionAsync(sessionId);
            await _db.SaveChangesAsync();
        }
    }

}
