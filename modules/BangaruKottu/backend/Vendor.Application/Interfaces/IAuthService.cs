using Vendor.Application.DTOs.Auth;

namespace Vendor.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);
    Task<bool> VerifyPasswordAsync(string password, string passwordHash);
}

