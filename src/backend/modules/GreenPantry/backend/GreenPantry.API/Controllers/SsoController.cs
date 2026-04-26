using GreenPantry.Application.DTOs.Auth;
using GreenPantry.Application.Interfaces;
using GreenPantry.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GreenPantry.Domain.Entities;
using GreenPantry.Domain.Enums;

namespace GreenPantry.API.Controllers;

public class SsoController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SsoController> _logger;

    public SsoController(
        IAuthService authService, 
        IUserRepository userRepository,
        IConfiguration configuration,
        ILogger<SsoController> logger)
    {
        _authService = authService;
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("sync")]
    public async Task<ActionResult<AuthResponse>> SyncSsoUser([FromBody] SsoSyncRequest request)
    {
        try
        {
            _logger.LogInformation("SSO Sync request received for email: {Email}", request.Email);

            // Check if user already exists
            var existingUser = await _userRepository.GetByEmailAsync(request.Email.ToLowerInvariant());

            if (existingUser != null)
            {
                _logger.LogInformation("User already exists, generating new token via login");
                
                // User exists - create a login request with a special SSO password
                // We'll use a deterministic password based on the user ID
                var ssoPassword = $"SSO_USER_{existingUser.Id}";
                
                // First, ensure the user's password is set to the SSO password
                await _authService.ResetPasswordDirectlyAsync(existingUser.Id.ToString(), ssoPassword);
                
                // Now login with that password
                var loginResponse = await _authService.LoginAsync(new LoginRequest
                {
                    Email = request.Email,
                    Password = ssoPassword
                });

                return Ok(loginResponse);
            }

            // User doesn't exist, create new user
            _logger.LogInformation("Creating new user from SSO");
            
            var ssoPasswordForNewUser = $"SSO_USER_{Guid.NewGuid()}";
            
            var registerRequest = new RegisterRequest
            {
                Email = request.Email,
                FirstName = request.FirstName ?? "User",
                LastName = request.LastName ?? "SSO",
                PhoneNumber = request.PhoneNumber ?? "0000000000",
                Password = ssoPasswordForNewUser,
                Role = request.Role ?? UserRole.User
            };

            var response = await _authService.RegisterAsync(registerRequest);
            _logger.LogInformation("User created successfully via SSO sync");
            
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during SSO sync for email: {Email}", request.Email);
            return StatusCode(500, new { message = "Failed to sync SSO user", error = ex.Message });
        }
    }
}

public class SsoSyncRequest
{
    public string Email { get; set; } = string.Empty;
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public UserRole? Role { get; set; }
}
