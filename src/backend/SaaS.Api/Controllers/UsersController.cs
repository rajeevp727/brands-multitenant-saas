using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SaaS.Application.DTOs;
using SaaS.Application.Common;
using SaaS.Domain.Entities;
using System.Security.Claims;

namespace SaaS.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IGenericService<UserDto, User> _userService;

    public UsersController(IGenericService<UserDto, User> userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userIdString = User.FindFirst("userId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

        if (!Guid.TryParse(userIdString, out var userId)) return BadRequest("Invalid User ID");

        var user = await _userService.GetByIdAsync(userId);
        if (user == null) return NotFound();

        return Ok(user);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe([FromBody] UserDto userDto)
    {
        var userIdString = User.FindFirst("userId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString)) return Unauthorized();

        if (!Guid.TryParse(userIdString, out var userId)) return BadRequest("Invalid User ID");

        await _userService.UpdateAsync(userId, userDto);
        return Ok(userDto);
    }

    [HttpGet("me/orders")]
    // [Authorize]
    public IActionResult GetMyOrders()
    {
        // Return mock orders for GreenPantry
        return Ok(new[]
        {
            new { id = "ord_001", date = DateTime.Now.AddDays(-1), total = 45.50, status = "Delivered", items = 3 },
            new { id = "ord_002", date = DateTime.Now.AddDays(-5), total = 120.00, status = "Processing", items = 8 }
        });
    }
}
