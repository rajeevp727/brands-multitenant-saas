using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using SaaS.Application.DTOs;
using SaaS.Application.Services;

namespace SaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IWebHostEnvironment _env;

    public AuthController(IAuthService authService, IWebHostEnvironment env)
    {
        _authService = authService;
        _env = env;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            
            // Issue Secure Cookie
            var cookieName = _env.IsDevelopment() ? "SaaS-Token" : "__Host-SaaS-Token";
            Response.Cookies.Append(cookieName, response.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = !_env.IsDevelopment(),
                SameSite = _env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            // Don't return the token in the response body to avoid localStorage storage
            return Ok(new { user = response.User });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        var cookieName = _env.IsDevelopment() ? "SaaS-Token" : "__Host-SaaS-Token";
        
        Response.Cookies.Delete(cookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = !_env.IsDevelopment(),
            SameSite = _env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict
        });
        
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return Unauthorized();
        }

        return Ok(new
        {
            userId = User.FindFirst("userId")?.Value,
            email = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value,
            username = User.FindFirst("username")?.Value,
            tenantId = User.FindFirst("tenantId")?.Value,
            role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            
            // Issue Secure Cookie on registration too
            var cookieName = _env.IsDevelopment() ? "SaaS-Token" : "__Host-SaaS-Token";
            Response.Cookies.Append(cookieName, response.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = !_env.IsDevelopment(),
                SameSite = _env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(new { user = response.User });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("login/{provider}")]
    public IActionResult ExternalLogin(string provider)
    {
        Serilog.Log.Information("Initiating external login for provider: {Provider}", provider);

        if (string.Equals(provider, "google", StringComparison.OrdinalIgnoreCase))
        {
            provider = Microsoft.AspNetCore.Authentication.Google.GoogleDefaults.AuthenticationScheme;
        }
        else if (string.Equals(provider, "facebook", StringComparison.OrdinalIgnoreCase))
        {
            provider = Microsoft.AspNetCore.Authentication.Facebook.FacebookDefaults.AuthenticationScheme;
        }
        else
        {
             Serilog.Log.Warning("Unsupported provider requested: {Provider}", provider);
             return BadRequest($"Provider '{provider}' is not supported.");
        }

        var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Auth");
        var properties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = redirectUrl };
        
        return Challenge(properties, provider);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> ExternalLoginCallback()
    {
        var result = await HttpContext.AuthenticateAsync(Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
        
        if (!result.Succeeded)
        {
             return BadRequest("External authentication failed.");
        }

        var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
        var email = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email)?.Value;
        var name = claims?.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(email))
        {
             return BadRequest("Email claim not received from provider.");
        }

        var user = await _authService.FindOrRegisterExternalUserAsync(email, name ?? email.Split('@')[0]);
        var tokenValue = _authService.GenerateTokenForUser(user);
        
        // Issue Secure Cookie
        var cookieName = _env.IsDevelopment() ? "SaaS-Token" : "__Host-SaaS-Token";
        Response.Cookies.Append(cookieName, tokenValue, new CookieOptions
        {
            HttpOnly = true,
            Secure = !_env.IsDevelopment(),
            SameSite = _env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });

        // Redirect to Frontend (Token is NOT in the URL anymore)
        var redirectUrl = _authService.GetFrontendRedirectUrl(tokenValue, email);
        return Redirect(redirectUrl);
    }
}
