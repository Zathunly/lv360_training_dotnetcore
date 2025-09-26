using lv360_training.Domain.Interfaces.Repositories.Core;
using lv360_training.Domain.Interfaces.Repositories.Auth;
using lv360_training.Domain.Entities;
using lv360_training.Domain.Dtos.Auth.Request;
using lv360_training.Domain.Enums;

public class AdminHandler
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    // private readonly IPermissionRepository _permissions;
    private readonly IUnitOfWork _uow;

    public AdminHandler(
        IUserRepository users,
        IRoleRepository roles,
        // IPermissionRepository permissions,
        IUnitOfWork uow)
    {
        _users = users;
        _roles = roles;
        // _permissions = permissions;
        _uow = uow;
    }

    public async Task AssignRoleAsync(string adminUsername, AssignRoleRequest request)
    {
        var admin = await _users.GetByUsernameAsync(adminUsername);
        if (admin == null || !await _users.HasRoleAsync(admin.Id, RoleNames.Admin))
            throw new Exception("Only admins can assign roles");

        if (admin.Id == request.UserID)
            throw new Exception("Cannot assign a role to yourself");

        var user = await _users.GetByIdAsync(request.UserID);
        if (user == null)
            throw new Exception("User not found");

        string roleName = request.Role switch
        {
            1 => RoleNames.Admin,
            2 => RoleNames.User,
            _ => throw new Exception("Invalid role value.")
        };

        var role = await _roles.GetByNameAsync(roleName);
        if (role == null)
            throw new Exception($"Role '{roleName}' not found");

        var currentRole = await _users.GetRoleByUserIdAsync(user.Id);

        if (currentRole != null)
        {
            if (currentRole.Id == role.Id)
            {
                return;
            }

            await _roles.RemoveUserRoleAsync(user.Id);
        }

        await _roles.AddUserRoleAsync(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        await _uow.SaveChangesAsync();
    }

    // public async Task AssignPermissionAsync(string adminUsername, AssignPermissionRequest request)
    // {
    //     var admin = await _users.GetByUsernameAsync(adminUsername);
    //     if (admin == null || !await _users.HasRoleAsync(admin.Id, "Admin"))
    //         throw new Exception("Only admins can assign permissions");

    //     var role = await _roles.GetByNameAsync(request.RoleName);
    //     if (role == null)
    //         throw new Exception("Role not found");

    //     var permission = await _permissions.GetByNameAsync(request.PermissionName);
    //     if (permission == null)
    //         throw new Exception("Permission not found");

    //     await _permissions.AddToRoleAsync(new RolePermission
    //     {
    //         RoleId = role.Id,
    //         PermissionId = permission.Id
    //     });

    //     await _uow.SaveChangesAsync();
    // }
}
