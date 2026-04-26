using Microsoft.AspNetCore.Mvc;
using Vendor.Application.DTOs.Auth;
using Vendor.Application.Interfaces;
using Vendor.Domain.Entities;
using Vendor.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Vendor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SsoController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IAuthService _authService;
    private readonly ILogger<SsoController> _logger;
    private readonly PasswordHasher<User> _passwordHasher = new();

    public SsoController(ApplicationDbContext context, IAuthService authService, ILogger<SsoController> logger)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("sync")]
    public async Task<ActionResult<LoginResponse>> SyncSsoUser([FromBody] SsoSyncRequest request)
    {
        try
        {
            _logger.LogInformation("SSO Sync request received for email: {Email}", request.Email);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            string ssoPassword = $"SSO_USER_{request.Email}";

            if (user == null)
            {
                _logger.LogInformation("Creating new user from SSO: {Email}", request.Email);
                user = new User
                {
                    Email = request.Email,
                    Username = request.Email, // Default username to email
                    Role = request.Role ?? "User",
                    PasswordHash = _passwordHasher.HashPassword(null!, ssoPassword)
                };
                await _context.Users.AddAsync(user);
            }
            else
            {
                _logger.LogInformation("Updating existing user from SSO: {Email}", request.Email);
                user.PasswordHash = _passwordHasher.HashPassword(user, ssoPassword);
                _context.Users.Update(user);
            }

            await _context.SaveChangesAsync();

            // Now perform login to get the token
            var loginResponse = await _authService.LoginAsync(new LoginRequest
            {
                Username = user.Username,
                Password = ssoPassword
            });

            if (loginResponse == null)
            {
                return Unauthorized(new { message = "Failed to generate token after sync" });
            }

            return Ok(loginResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during SSO sync for email: {Email}", request.Email);
            return StatusCode(500, new { message = "Failed to sync SSO user", error = ex.Message });
        }
    }
}
