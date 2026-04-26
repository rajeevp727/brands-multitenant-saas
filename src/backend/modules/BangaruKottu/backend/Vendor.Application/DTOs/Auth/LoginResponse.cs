namespace Vendor.Application.DTOs.Auth;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int VendorId { get; set; }
    public string VendorName { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}

