namespace Vendor.Application.DTOs.Product;

public class CreateProductRequest
{
    public int CategoryId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int? StockQuantity { get; set; }
    public string? ImageUrl { get; set; }
}

