using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GreenPantry.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public abstract class BaseApiController : ControllerBase
{
    protected string? CurrentUserId => 
        User.FindFirst("userId")?.Value ?? 
        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

    protected string? CurrentUserRole => 
        User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? 
        User.FindFirst("role")?.Value;

    protected bool IsUserInRole(string role) => CurrentUserRole == role;
    protected bool IsAdmin => IsUserInRole("Admin");
}
