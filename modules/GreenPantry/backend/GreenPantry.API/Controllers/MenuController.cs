using GreenPantry.Application.DTOs.Menu;
using GreenPantry.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreenPantry.API.Controllers;

public class MenuController : BaseApiController
{
    private readonly IMenuService _menuService;

    public MenuController(IMenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet("restaurant/{restaurantId}")]
    public async Task<ActionResult<IEnumerable<MenuCategoryDto>>> GetMenuByRestaurant(string restaurantId)
    {
        var menu = await _menuService.GetMenuByRestaurantIdAsync(restaurantId);
        return Ok(menu);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MenuItemDto>> GetMenuItem(string id)
    {
        var menuItem = await _menuService.GetMenuItemByIdAsync(id);
        if (menuItem == null)
        {
            return NotFound();
        }

        return Ok(menuItem);
    }

    [HttpPost]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<ActionResult<MenuItemDto>> CreateMenuItem(MenuItemDto menuItem)
    {
        var createdMenuItem = await _menuService.CreateMenuItemAsync(menuItem);
        return CreatedAtAction(nameof(GetMenuItem), new { id = createdMenuItem.Id }, createdMenuItem);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<ActionResult<MenuItemDto>> UpdateMenuItem(string id, MenuItemDto menuItem)
    {
        var updatedMenuItem = await _menuService.UpdateMenuItemAsync(id, menuItem);
        return Ok(updatedMenuItem);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<ActionResult> DeleteMenuItem(string id)
    {
        var success = await _menuService.DeleteMenuItemAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}
