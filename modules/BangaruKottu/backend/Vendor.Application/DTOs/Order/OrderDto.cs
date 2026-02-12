namespace Vendor.Application.DTOs.Order;

public class OrderDto
{
    public int OrderId { get; set; }
    public string OrderStatus { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string? CustomerName { get; set; }
    public string? CustomerPhone { get; set; }
    public string? DeliveryAddress { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? PreparedAt { get; set; }
    public DateTime? DispatchedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public List<OrderItemDto> OrderItems { get; set; } = new();
}

