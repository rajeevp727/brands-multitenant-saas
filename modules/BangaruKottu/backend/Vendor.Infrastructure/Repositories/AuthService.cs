using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Vendor.Application.DTOs.Auth;
using Vendor.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Vendor.Domain.Entities;
using Vendor.Infrastructure.Data;
// using BCrypt.Net;

namespace Vendor.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public AuthService(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Username == request.Username);

        if (user == null)
            return null;

        var isValidPassword = await VerifyPasswordAsync(request.Password, user.PasswordHash);
        if (!isValidPassword)
            return null;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Role, user.Role ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = token.ValidTo
        };
    }

    public Task<bool> VerifyPasswordAsync(string password, string passwordHash)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            null!,
            passwordHash,
            password
        );

        return Task.FromResult(result == PasswordVerificationResult.Success);
    }
}
