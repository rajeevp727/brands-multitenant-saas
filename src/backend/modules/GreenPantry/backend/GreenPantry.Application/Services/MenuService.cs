using Mapster;
using GreenPantry.Application.DTOs.Menu;
using GreenPantry.Application.Interfaces;
using GreenPantry.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace GreenPantry.Application.Services;

public class MenuService : IMenuService
{
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ILogger<MenuService> _logger;

    public MenuService(
        IMenuItemRepository menuItemRepository,
        ILogger<MenuService> logger)
    {
        _menuItemRepository = menuItemRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<MenuCategoryDto>> GetMenuByRestaurantIdAsync(string restaurantId)
    {
        _logger.LogInformation("Getting menu for restaurant: {RestaurantId}", restaurantId);

        var menuItems = await _menuItemRepository.GetByRestaurantIdAsync(Guid.Parse(restaurantId));
        var availableItems = menuItems.Where(m => m.IsAvailable && !m.IsDeleted);

        var groupedItems = availableItems
            .GroupBy(m => m.Category)
            .Select(g => new MenuCategoryDto
            {
                Category = g.Key,
                Items = g.ToList().Adapt<List<MenuItemDto>>()
            })
            .OrderBy(c => c.Category);

        return groupedItems;
    }

    public async Task<MenuItemDto?> GetMenuItemByIdAsync(string id)
    {
        _logger.LogInformation("Getting menu item by ID: {MenuItemId}", id);

        var menuItem = await _menuItemRepository.GetByIdAsync(Guid.Parse(id));
        if (menuItem == null || menuItem.IsDeleted)
        {
            return null;
        }

        return menuItem.Adapt<MenuItemDto>();
    }

    public async Task<MenuItemDto> CreateMenuItemAsync(MenuItemDto menuItem)
    {
        _logger.LogInformation("Creating new menu item: {MenuItemName}", menuItem.Name);

        var menuItemEntity = menuItem.Adapt<MenuItem>();
        menuItemEntity.CreatedAt = DateTime.UtcNow;
        menuItemEntity.UpdatedAt = DateTime.UtcNow;

        var createdMenuItem = await _menuItemRepository.CreateAsync(menuItemEntity);
        return createdMenuItem.Adapt<MenuItemDto>();
    }

    public async Task<MenuItemDto> UpdateMenuItemAsync(string id, MenuItemDto menuItem)
    {
        _logger.LogInformation("Updating menu item: {MenuItemId}", id);

        var existingMenuItem = await _menuItemRepository.GetByIdAsync(Guid.Parse(id));
        if (existingMenuItem == null)
        {
            throw new KeyNotFoundException($"Menu item with ID {id} not found");
        }

        menuItem.Adapt(existingMenuItem);
        existingMenuItem.UpdatedAt = DateTime.UtcNow;

        var updatedMenuItem = await _menuItemRepository.UpdateAsync(existingMenuItem);
        return updatedMenuItem.Adapt<MenuItemDto>();
    }

    public async Task<bool> DeleteMenuItemAsync(string id)
    {
        _logger.LogInformation("Deleting menu item: {MenuItemId}", id);

        var menuItem = await _menuItemRepository.GetByIdAsync(Guid.Parse(id));
        if (menuItem == null)
        {
            return false;
        }

        menuItem.IsDeleted = true;
        menuItem.UpdatedAt = DateTime.UtcNow;
        await _menuItemRepository.UpdateAsync(menuItem);

        return true;
    }

    public async Task<IEnumerable<MenuItemDto>> GetMenuItemsByRestaurantAsync(string restaurantId)
    {
        _logger.LogInformation("Getting menu items for restaurant: {RestaurantId}", restaurantId);

        var menuItems = await _menuItemRepository.GetByRestaurantIdAsync(Guid.Parse(restaurantId));
        var availableItems = menuItems.Where(m => !m.IsDeleted);

        return availableItems.Adapt<IEnumerable<MenuItemDto>>();
    }
}
