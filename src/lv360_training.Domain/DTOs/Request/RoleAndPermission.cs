// AssignRoleRequest.cs
namespace lv360_training.Domain.Dtos.Auth.Request;

public class AssignRoleRequest
{
    public int UserID { get; set; }
    public int Role { get; set; }
}

// // AssignPermissionRequest.cs
// public class AssignPermissionRequest
// {
//     public string RoleName { get; set; } = null!;
//     public string PermissionName { get; set; } = null!;
// }
