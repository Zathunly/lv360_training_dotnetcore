using Microsoft.AspNetCore.Mvc;
using lv360_training.Application.Handlers;
using lv360_training.Domain.Dtos.Auth.Request;
using lv360_training.Domain.Dtos.Auth.Response;
using lv360_training.Domain.Enums;
using lv360_training.Infrastructure.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace lv360_training.Api.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthHandler _authHandler;

    public AuthController(AuthHandler authHandler)
    {
        _authHandler = authHandler;
    }

    [HttpPost("register/user")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest request)
    {
        try
        {
            var userId = await _authHandler.Register(request, RoleNames.User);
            return Ok(new { Id = userId, Role = RoleNames.User });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _authHandler.ValidateUserAsync(request);
        if (user == null)
            return Unauthorized(new { error = "Invalid credentials" });

        var (sessionId, session) = await _authHandler.CreateSessionAsync(user);
        var principal = ClaimsHelper.CreateClaimsPrincipal(user, sessionId, session);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = session.ExpiresAt
            }
        );

        var roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>();
        return Ok(new
        {
            message = "Successfully Logged In",
            username = user.Username,
            expiresAt = session.ExpiresAt,
            roles
        });
    }


    [HttpGet("me")]
    [Authorize] 
    public async Task<IActionResult> Me()
    {
        var validSession = await _authHandler.ValidateSessionAsync(User);
        if (!validSession)
            return Unauthorized(new { error = "Session expired" });

        var user = await _authHandler.GetUserFromClaimsAsync(User);
        if (user == null)
            return Unauthorized(new { error = "User not found" });

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        var authResult = await HttpContext.AuthenticateAsync();
        DateTime? expiresAt = authResult?.Properties?.ExpiresUtc?.UtcDateTime;

        var response = new MeResponse
        {
            Username = user.Username,
            Roles = roles,
            ExpiresAt = expiresAt
        };

        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authHandler.LogoutUserAsync(User); 
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Successfully Logged Out" });
    }
}
