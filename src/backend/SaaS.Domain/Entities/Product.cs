using SaaS.Domain.Common;

namespace SaaS.Domain.Entities;

public class Product : AuditableEntity<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int StockQuantity { get; set; } = -1; // -1 for unlimited
    public bool IsActive { get; set; } = true;
    public string MetadataJson { get; set; } = "{}"; // For business-specific fields (e.g., calories, weight, dimensions)
}
