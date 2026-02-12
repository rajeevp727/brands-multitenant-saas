namespace Vendor.Domain.Entities;

public class VendorEntity
{
    public int VendorId { get; set; }
    public int UserId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string? ContactNumber { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? CreatedAt { get; set; }

    public User? User { get; set; }
}

