namespace SaaS.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime OrderDate { get; set; }
    public string? ShippingAddress { get; set; }
    public string? Notes { get; set; }
    public string MetadataJson { get; set; } = "{}";
    public DateTime CreatedAt { get; set; }
}
