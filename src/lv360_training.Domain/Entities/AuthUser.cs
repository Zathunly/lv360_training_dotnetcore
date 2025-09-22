using System.ComponentModel.DataAnnotations;

namespace lv360_training.Domain.Entities;

public class User
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Username { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    public string? Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Relations
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class Role
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;
    
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class Permission
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; } = null!;
    
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}

public class UserRole
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
}

public class RolePermission
{
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
}

public class Session
{
    [Key]
    public Guid Id { get; set; }
    
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
