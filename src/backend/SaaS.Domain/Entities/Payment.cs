using SaaS.Domain.Common;

namespace SaaS.Domain.Entities;

public class Payment : AuditableEntity<Guid>
{
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;

    public virtual Order? Order { get; set; }
}
