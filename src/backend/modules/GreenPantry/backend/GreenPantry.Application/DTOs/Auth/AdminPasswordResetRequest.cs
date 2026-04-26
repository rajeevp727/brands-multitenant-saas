namespace GreenPantry.Application.DTOs.Auth;

public class AdminPasswordResetRequest
{
    public string Email { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
