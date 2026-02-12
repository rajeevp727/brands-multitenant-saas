using GreenPantry.Application.DTOs.Restaurant;
using GreenPantry.Application.DTOs.Menu;
using GreenPantry.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace GreenPantry.API.Controllers;

public class RestaurantsController : BaseApiController
{
    private readonly IRestaurantService _restaurantService;
    private readonly IMenuService _menuService;

    public RestaurantsController(IRestaurantService restaurantService, IMenuService menuService)
    {
        _restaurantService = restaurantService;
        _menuService = menuService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RestaurantDto>>> GetRestaurants([FromQuery] RestaurantFilterDto filter)
    {
        var restaurants = await _restaurantService.GetRestaurantsAsync(filter);
        return Ok(restaurants);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RestaurantDetailDto>> GetRestaurant(string id)
    {
        var restaurant = await _restaurantService.GetRestaurantByIdAsync(id);
        if (restaurant == null)
        {
            return NotFound();
        }

        return Ok(restaurant);
    }

    [HttpGet("{id}/menu")]
    public async Task<ActionResult<IEnumerable<MenuCategoryDto>>> GetRestaurantMenu(string id)
    {
        var menu = await _menuService.GetMenuByRestaurantIdAsync(id);
        return Ok(menu);
    }

    [HttpPost]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<ActionResult<RestaurantDto>> CreateRestaurant(RestaurantDto restaurant)
    {
        var createdRestaurant = await _restaurantService.CreateRestaurantAsync(restaurant);
        return CreatedAtAction(nameof(GetRestaurant), new { id = createdRestaurant.Id }, createdRestaurant);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<ActionResult<RestaurantDto>> UpdateRestaurant(string id, RestaurantDto restaurant)
    {
        var updatedRestaurant = await _restaurantService.UpdateRestaurantAsync(id, restaurant);
        return Ok(updatedRestaurant);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Vendor,Admin")]
    public async Task<ActionResult> DeleteRestaurant(string id)
    {
        var success = await _restaurantService.DeleteRestaurantAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}
