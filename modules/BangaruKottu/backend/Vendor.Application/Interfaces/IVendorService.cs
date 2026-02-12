using Vendor.Application.DTOs.Vendor;

namespace Vendor.Application.Interfaces;

public interface IVendorService
{
    Task<VendorProfileDto?> GetVendorProfileAsync(int vendorId);
    Task<VendorProfileDto?> UpdateVendorProfileAsync(int vendorId, UpdateVendorRequest request);
}

