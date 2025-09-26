using Microsoft.AspNetCore.Mvc;
using lv360_training.Application.Handlers;
using lv360_training.Domain.Dtos.Auth.Request;
using lv360_training.Domain.Enums;
using lv360_training.Infrastructure.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

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

        var session = await _authHandler.CreateSessionAsync(user);
        var principal = ClaimsHelper.CreateClaimsPrincipal(user, session);

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
            expiresAt = session.ExpiresAt 
        });
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authHandler.LogoutUserAsync(User); 
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Successfully Logged Out" });
    }
}
