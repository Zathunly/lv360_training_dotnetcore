// AssignRoleRequest.cs
namespace lv360_training.Application.Dtos.Auth.Request;

public class AssignRoleRequest
{
    public string Username { get; set; } = null!;
    public string RoleName { get; set; } = null!;
}

// AssignPermissionRequest.cs
public class AssignPermissionRequest
{
    public string RoleName { get; set; } = null!;
    public string PermissionName { get; set; } = null!;
}
