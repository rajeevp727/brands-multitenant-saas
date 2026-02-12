using SaaS.Domain.Common;

namespace SaaS.Domain.Entities;

public class AppConstant : BaseEntity<Guid>
{
    public string? TenantId { get; set; }
    public string ConstantKey { get; set; } = string.Empty;
    public string ConstantValue { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
}
