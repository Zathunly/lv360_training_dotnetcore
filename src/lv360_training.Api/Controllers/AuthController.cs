using Microsoft.AspNetCore.Mvc;
using lv360_training.Application.Handlers;
using lv360_training.Api.Dtos.Auth;
using lv360_training.Application.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace lv360_training.Api.Controllers;

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

        // --- Create DB session ---
        var session = await _authHandler.CreateSessionAsync(user);

        // --- Load user roles from database ---
        var roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>();

        // --- Create claims ---
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("SessionId", session.Id.ToString())
        };

        // Add a claim for each role
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = session.ExpiresAt
            }
        );

        return Ok(new 
        { 
            message = "Successfully Logged In", 
            username = user.Username, 
            roles = roles,
            expiresAt = session.ExpiresAt 
        });
    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _authHandler.LogoutUserAsync(User); // ClaimsPrincipal comes from ControllerBase.User
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Successfully Logged Out" });
    }
}
