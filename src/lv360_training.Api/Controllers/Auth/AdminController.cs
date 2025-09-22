using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lv360_training.Api.Controllers.Auth;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")] 
public class AdminController : ControllerBase
{
    private readonly AdminHandler _handler;

    public AdminController(AdminHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("resource")]
    public IActionResult GetAdminResource()
    {
        return Ok(new { message = "Admin Resource" });
    }

    // [HttpPost("assign-role")]
    // public async Task<IActionResult> AssignRole([FromBody] AssignRoleRequest request)
    // {
    //     try
    //     {
    //         var adminUsername = User.Identity!.Name!;
    //         await _handler.AssignRoleAsync(adminUsername, request);
    //         return Ok(new { message = $"Role '{request.RoleName}' assigned to '{request.Username}'" });
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(new { error = ex.Message });
    //     }
    // }

    // [HttpPost("assign-permission")]
    // public async Task<IActionResult> AssignPermission([FromBody] AssignPermissionRequest request)
    // {
    //     try
    //     {
    //         var adminUsername = User.Identity!.Name!;
    //         await _handler.AssignPermissionAsync(adminUsername, request);
    //         return Ok(new { message = $"Permission '{request.PermissionName}' assigned to role '{request.RoleName}'" });
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(new { error = ex.Message });
    //     }
    // }
}
