using System.Security.Claims;

namespace Vendor.API.Extensions;

public static class ClaimsExtensions
{
    public static int GetVendorId(this ClaimsPrincipal user)
    {
        var vendorIdClaim = user.FindFirst("VendorId")?.Value;
        if (int.TryParse(vendorIdClaim, out var vendorId))
            return vendorId;
        throw new UnauthorizedAccessException("Vendor ID not found in token");
    }

    public static int GetUserId(this ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
            return userId;
        throw new UnauthorizedAccessException("User ID not found in token");
    }
}

