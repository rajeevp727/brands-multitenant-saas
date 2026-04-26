namespace Vendor.Application.DTOs.Vendor;

public class UpdateVendorRequest
{
    public string VendorName { get; set; } = string.Empty;
    public string? ContactNumber { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
}

