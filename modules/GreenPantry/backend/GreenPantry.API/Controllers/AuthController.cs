using GreenPantry.Application.DTOs.Auth;
using GreenPantry.Application.Interfaces;
using GreenPantry.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace GreenPantry.API.Controllers;

public class AuthController : BaseApiController
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepository;

    public AuthController(IAuthService authService, IUserRepository userRepository)
    {
        _authService = authService;
        _userRepository = userRepository;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = await _authService.RefreshTokenAsync(request.RefreshToken);
        return Ok(response);
    }

    [HttpPost("logout")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<ActionResult> Logout()
    {
        if (string.IsNullOrEmpty(CurrentUserId))
        {
            return Unauthorized();
        }

        await _authService.LogoutAsync(CurrentUserId);
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        await _authService.RequestPasswordResetAsync(request.Email);
        return Ok(new { message = "If an account with that email exists, a password reset link has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var success = await _authService.ResetPasswordAsync(request.UserId, request.Token, request.NewPassword);
        
        if (!success)
        {
            return BadRequest(new { message = "Invalid or expired reset token" });
        }

        return Ok(new { message = "Password reset successful" });
    }

    [HttpGet("test-users")]
    public async Task<ActionResult> GetTestUsers()
    {
        var users = await _userRepository.GetAllAsync();
        var userEmails = users.Select(u => new { u.Email, u.Id }).ToList();
        return Ok(userEmails);
    }

    [HttpPost("admin/reset-password")]
    public async Task<ActionResult> AdminResetPassword([FromBody] AdminPasswordResetRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email.ToLowerInvariant());
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        await _authService.ResetPasswordDirectlyAsync(user.Id.ToString(), request.NewPassword);
        return Ok(new { message = "Password reset successfully", newPassword = request.NewPassword });
    }

    [HttpPost("fix-password-by-id")]
    public async Task<ActionResult> FixPasswordById([FromBody] JsonElement request)
    {
        var requestString = request.GetRawText();
        if (string.IsNullOrEmpty(requestString))
        {
            return BadRequest(new { message = "Request body is empty" });
        }

        var requestDict = JsonSerializer.Deserialize<Dictionary<string, object>>(requestString);
        if (requestDict == null)
        {
            return BadRequest(new { message = "Invalid request format" });
        }

        var userId = requestDict.ContainsKey("userId") ? requestDict["userId"]?.ToString() : null;
        var newPassword = requestDict.ContainsKey("newPassword") ? requestDict["newPassword"]?.ToString() : null;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(newPassword))
        {
            return BadRequest(new { message = "userId and newPassword are required" });
        }

        await _authService.ResetPasswordDirectlyAsync(userId, newPassword);
        
        return Ok(new { 
            message = "Password fixed successfully", 
            userId = userId
        });
    }
}
