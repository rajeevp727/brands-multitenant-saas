using GreenPantry.Application.Interfaces;
using GreenPantry.Domain.Entities;
using GreenPantry.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GreenPantry.Infrastructure.Repositories;

public class MenuItemRepository : BaseRepository<MenuItem>, IMenuItemRepository
{
    public MenuItemRepository(AppDbContext context, ILogger<MenuItemRepository> logger) 
        : base(context, logger)
    {
    }

    public async Task<IEnumerable<MenuItem>> GetByRestaurantIdAsync(Guid restaurantId)
    {
        try
        {
            return await _dbSet.Where(m => m.RestaurantId == restaurantId && !m.IsDeleted).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting menu items by restaurant: {RestaurantId}", restaurantId);
            throw;
        }
    }

    public new async Task<MenuItem> CreateAsync(MenuItem menuItem)
    {
        return await base.CreateAsync(menuItem);
    }

    public new async Task<MenuItem> UpdateAsync(MenuItem menuItem)
    {
        return await base.UpdateAsync(menuItem);
    }

    public override async Task<MenuItem?> GetByIdAsync(Guid id)
    {
        return await base.GetByIdAsync(id);
    }

    public override async Task<bool> DeleteAsync(Guid id)
    {
        return await base.DeleteAsync(id);
    }
}
