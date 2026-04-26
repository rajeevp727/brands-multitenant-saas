using GreenPantry.Application.Interfaces;
using GreenPantry.Domain.Entities;
using GreenPantry.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GreenPantry.Infrastructure.Repositories;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(AppDbContext context, ILogger<OrderRepository> logger) 
        : base(context, logger)
    {
    }

    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId)
    {
        try
        {
            return await _dbSet
                .Where(o => o.UserId == userId && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders by user: {UserId}", userId);
            throw;
        }
    }

    public async Task<IEnumerable<Order>> GetByRestaurantIdAsync(Guid restaurantId)
    {
        try
        {
            return await _dbSet
                .Where(o => o.RestaurantId == restaurantId && !o.IsDeleted)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting orders by restaurant: {RestaurantId}", restaurantId);
            throw;
        }
    }

    public new async Task<Order> CreateAsync(Order order)
    {
        return await base.CreateAsync(order);
    }

    public new async Task<Order> UpdateAsync(Order order)
    {
        return await base.UpdateAsync(order);
    }

    public override async Task<Order?> GetByIdAsync(Guid id)
    {
        return await base.GetByIdAsync(id);
    }

    public override async Task<bool> DeleteAsync(Guid id)
    {
        return await base.DeleteAsync(id);
    }
}
