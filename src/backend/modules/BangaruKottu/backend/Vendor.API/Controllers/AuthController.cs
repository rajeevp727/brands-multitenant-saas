using Microsoft.AspNetCore.Mvc;
using Vendor.API.Extensions;
using Vendor.Application.DTOs.Auth;
using Vendor.Application.Interfaces;

namespace Vendor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (response == null)
            return Unauthorized(new { message = "Invalid email or password" });

        return Ok(response);
    }
}

