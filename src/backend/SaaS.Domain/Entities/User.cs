using System.ComponentModel.DataAnnotations.Schema;
using SaaS.Domain.Common;

namespace SaaS.Domain.Entities;

public class User : AuditableEntity<Guid>
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public Guid? RoleId { get; set; }
    public bool IsActive { get; set; } = true;
    public string? PhoneNumber { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? StreetAddress { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public bool IsEmailVerified { get; set; } = false;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiryTime { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    [Column("Role")]
    public int UserRoleValue { get; set; } = 0;

    public virtual Role? Role { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}

public class Role : BaseEntity<Guid>, ITenantEntity
{
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
