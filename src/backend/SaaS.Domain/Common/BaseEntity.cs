namespace SaaS.Domain.Common;

public abstract class BaseEntity<TId>
{
    public TId Id { get; set; } = default!;
}

public interface ITenantEntity
{
    public string TenantId { get; set; }
}

public abstract class AuditableEntity<TId> : BaseEntity<TId>, ITenantEntity
{
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string CreatedBy { get; set; } = "System";
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
}
