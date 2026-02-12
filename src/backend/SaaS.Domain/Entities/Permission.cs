using SaaS.Domain.Common;

namespace SaaS.Domain.Entities;

public class Permission : BaseEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}

public class FeatureFlag : BaseEntity<Guid>, ITenantEntity
{
    public string TenantId { get; set; } = string.Empty;
    public string FeatureKey { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
