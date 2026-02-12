using GreenPantry.Application.DTOs.Auth;
using GreenPantry.Application.Interfaces;
using GreenPantry.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenPantry.API.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        if (string.IsNullOrEmpty(CurrentUserId))
        {
            return Unauthorized();
        }

        var user = await _userService.GetUserByIdAsync(CurrentUserId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserDto>> UpdateCurrentUser(UserDto userDto)
    {
        if (string.IsNullOrEmpty(CurrentUserId))
        {
            return Unauthorized();
        }

        var updatedUser = await _userService.UpdateUserProfileAsync(CurrentUserId, userDto);
        return Ok(updatedUser);
    }

    [HttpGet("me/orders")]
    public async Task<ActionResult<IEnumerable<GreenPantry.Application.DTOs.Order.OrderDto>>> GetMyOrderHistory()
    {
        if (string.IsNullOrEmpty(CurrentUserId))
        {
            return Unauthorized();
        }

        var orders = await _userService.GetUserOrderHistoryAsync(CurrentUserId);
        return Ok(orders);
    }

    [HttpPut("me/address")]
    public async Task<ActionResult> UpdateMyAddress(AddressDto addressDto)
    {
        if (string.IsNullOrEmpty(CurrentUserId))
        {
            return Unauthorized();
        }

        var success = await _userService.UpdateUserAddressAsync(CurrentUserId, addressDto);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}
