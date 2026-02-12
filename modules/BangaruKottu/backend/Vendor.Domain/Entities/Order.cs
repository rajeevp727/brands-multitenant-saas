namespace Vendor.Domain.Entities;

public class Order
{
    public int OrderId { get; set; }
    public int VendorId { get; set; }
    public string OrderStatus { get; set; } = "Pending";
    public decimal TotalAmount { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? DeliveryAddress { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? AcceptedAt { get; set; }
    public DateTime? PreparedAt { get; set; }
    public DateTime? DispatchedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }

    public VendorEntity? Vendor { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}

