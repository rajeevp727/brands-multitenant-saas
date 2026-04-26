using GreenPantry.Application.Interfaces;
using GreenPantry.Domain.Entities;
using GreenPantry.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GreenPantry.Infrastructure.Repositories;

public class RestaurantRepository : BaseRepository<Restaurant>, IRestaurantRepository
{
    public RestaurantRepository(AppDbContext context, ILogger<RestaurantRepository> logger) 
        : base(context, logger)
    {
    }

    public async Task<IEnumerable<Restaurant>> GetByOwnerIdAsync(Guid ownerId)
    {
        try
        {
            return await _dbSet.Where(r => r.OwnerId == ownerId && !r.IsDeleted).ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting restaurants by owner: {OwnerId}", ownerId);
            throw;
        }
    }

    public new async Task<IEnumerable<Restaurant>> GetAllAsync()
    {
        return await base.GetAllAsync();
    }

    public new async Task<Restaurant> CreateAsync(Restaurant restaurant)
    {
        return await base.CreateAsync(restaurant);
    }

    public new async Task<Restaurant> UpdateAsync(Restaurant restaurant)
    {
        return await base.UpdateAsync(restaurant);
    }

    public override async Task<Restaurant?> GetByIdAsync(Guid id)
    {
        return await base.GetByIdAsync(id);
    }

    public override async Task<bool> DeleteAsync(Guid id)
    {
        return await base.DeleteAsync(id);
    }
}
