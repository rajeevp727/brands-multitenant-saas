namespace Vendor.Domain.Entities;

public class Product
{
    public int ProductId { get; set; }
    public int VendorId { get; set; }
    public int CategoryId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int? StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? CreatedAt { get; set; }
    public string? ImageUrl { get; set; }

    public VendorEntity? Vendor { get; set; }
    public Category? Category { get; set; }
}

