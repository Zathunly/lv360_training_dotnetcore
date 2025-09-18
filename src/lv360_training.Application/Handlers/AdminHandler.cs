using lv360_training.Application.Interfaces.Repositories.Core;
using lv360_training.Application.Interfaces.Repositories.Auth;
using lv360_training.Domain;

public class AdminHandler
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IPermissionRepository _permissions;
    private readonly IUnitOfWork _uow;

    public AdminHandler(
        IUserRepository users,
        IRoleRepository roles,
        IPermissionRepository permissions,
        IUnitOfWork uow)
    {
        _users = users;
        _roles = roles;
        _permissions = permissions;
        _uow = uow;
    }

    public async Task AssignRoleAsync(string adminUsername, AssignRoleRequest request)
    {
        var admin = await _users.GetByUsernameAsync(adminUsername);
        if (admin == null || !await _users.HasRoleAsync(admin.Id, "Admin"))
            throw new Exception("Only admins can assign roles");

        var user = await _users.GetByUsernameAsync(request.Username);
        if (user == null)
            throw new Exception("User not found");

        var role = await _roles.GetByNameAsync(request.RoleName);
        if (role == null)
            throw new Exception("Role not found");

        await _roles.AddUserRoleAsync(new UserRole
        {
            UserId = user.Id,
            RoleId = role.Id
        });

        await _uow.SaveChangesAsync();
    }

    public async Task AssignPermissionAsync(string adminUsername, AssignPermissionRequest request)
    {
        var admin = await _users.GetByUsernameAsync(adminUsername);
        if (admin == null || !await _users.HasRoleAsync(admin.Id, "Admin"))
            throw new Exception("Only admins can assign permissions");

        var role = await _roles.GetByNameAsync(request.RoleName);
        if (role == null)
            throw new Exception("Role not found");

        var permission = await _permissions.GetByNameAsync(request.PermissionName);
        if (permission == null)
            throw new Exception("Permission not found");

        await _permissions.AddToRoleAsync(new RolePermission
        {
            RoleId = role.Id,
            PermissionId = permission.Id
        });

        await _uow.SaveChangesAsync();
    }
}
