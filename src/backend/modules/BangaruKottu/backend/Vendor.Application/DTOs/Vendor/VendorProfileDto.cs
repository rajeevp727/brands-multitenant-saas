namespace Vendor.Application.DTOs.Vendor;

public class VendorProfileDto
{
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public string? ContactNumber { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string Email { get; set; } = string.Empty;
}

