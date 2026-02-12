using AutoMapper;
using Vendor.Application.DTOs.Vendor;
using Vendor.Application.Interfaces;

namespace Vendor.Application.Services;

public class VendorService : IVendorService
{
    private readonly IVendorRepository _vendorRepository;
    private readonly IMapper _mapper;

    public VendorService(IVendorRepository vendorRepository, IMapper mapper)
    {
        _vendorRepository = vendorRepository;
        _mapper = mapper;
    }

    public async Task<VendorProfileDto?> GetVendorProfileAsync(int vendorId)
    {
        var vendor = await _vendorRepository.GetVendorWithUserAsync(vendorId);
        if (vendor == null)
            return null;

        return _mapper.Map<VendorProfileDto>(vendor);
    }

    public async Task<VendorProfileDto?> UpdateVendorProfileAsync(int vendorId, UpdateVendorRequest request)
    {
        var vendor = await _vendorRepository.GetByIdAsync(vendorId);
        if (vendor == null)
            return null;

        vendor.VendorName = request.VendorName;
        vendor.ContactNumber = request.ContactNumber;
        vendor.Address = request.Address;
        vendor.Description = request.Description;

        await _vendorRepository.UpdateAsync(vendor);

        var updatedVendor = await _vendorRepository.GetVendorWithUserAsync(vendorId);
        return _mapper.Map<VendorProfileDto>(updatedVendor);
    }
}

