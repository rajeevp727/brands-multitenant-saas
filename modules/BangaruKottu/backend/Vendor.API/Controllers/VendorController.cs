using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vendor.API.Extensions;
using Vendor.Application.DTOs.Vendor;
using Vendor.Application.Interfaces;

namespace Vendor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Vendor")]
public class VendorController : ControllerBase
{
    private readonly IVendorService _vendorService;
    private readonly ILogger<VendorController> _logger;

    public VendorController(IVendorService vendorService, ILogger<VendorController> logger)
    {
        _vendorService = vendorService;
        _logger = logger;
    }

    [HttpGet("profile")]
    [ProducesResponseType(typeof(VendorProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorProfileDto>> GetProfile()
    {
        var vendorId = User.GetVendorId();
        var profile = await _vendorService.GetVendorProfileAsync(vendorId);
        if (profile == null)
            return NotFound(new { message = "Vendor profile not found" });

        return Ok(profile);
    }

    [HttpPut("profile")]
    [ProducesResponseType(typeof(VendorProfileDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VendorProfileDto>> UpdateProfile([FromBody] UpdateVendorRequest request)
    {
        var vendorId = User.GetVendorId();
        var profile = await _vendorService.UpdateVendorProfileAsync(vendorId, request);
        if (profile == null)
            return NotFound(new { message = "Vendor profile not found" });

        return Ok(profile);
    }
}

