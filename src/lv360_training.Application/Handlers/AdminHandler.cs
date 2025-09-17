using lv360_training.Application.Interfaces;
using lv360_training.Domain;

public class AdminHandler
{
    private readonly IDbService _db;

    public AdminHandler(IDbService db)
    {
        _db = db;
    }

    public async Task AssignRoleAsync(string adminUsername, AssignRoleRequest request)
    {
        var admin = await _db.GetUserByUsernameAsync(adminUsername);
        if (admin == null || !await _db.UserHasRoleAsync(admin.Id, "Admin"))
            throw new Exception("Only admins can assign roles");

        var user = await _db.GetUserByUsernameAsync(request.Username);
        if (user == null)
            throw new Exception("User not found");

        var role = await _db.GetRoleByNameAsync(request.RoleName);
        if (role == null)
            throw new Exception("Role not found");

        await _db.AddUserRoleAsync(new UserRole { UserId = user.Id, RoleId = role.Id });
        await _db.SaveChangesAsync();
    }

    public async Task AssignPermissionAsync(string adminUsername, AssignPermissionRequest request)
    {
        var admin = await _db.GetUserByUsernameAsync(adminUsername);
        if (admin == null || !await _db.UserHasRoleAsync(admin.Id, "Admin"))
            throw new Exception("Only admins can assign permissions");

        var role = await _db.GetRoleByNameAsync(request.RoleName);
        if (role == null)
            throw new Exception("Role not found");

        var permission = await _db.GetPermissionByNameAsync(request.PermissionName);
        if (permission == null)
            throw new Exception("Permission not found");

        await _db.AddRolePermissionAsync(new RolePermission { RoleId = role.Id, PermissionId = permission.Id });
        await _db.SaveChangesAsync();
    }
}
