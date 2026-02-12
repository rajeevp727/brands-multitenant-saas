using SaaS.Domain.Common;

namespace SaaS.Domain.Entities;

public class Order : AuditableEntity<Guid>
{
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public string? ShippingAddress { get; set; }
    public string? Notes { get; set; }
    public string MetadataJson { get; set; } = "{}"; // For custom order data

    public virtual User? User { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class OrderItem : AuditableEntity<Guid>
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public virtual Order? Order { get; set; }
    public virtual Product? Product { get; set; }
}
