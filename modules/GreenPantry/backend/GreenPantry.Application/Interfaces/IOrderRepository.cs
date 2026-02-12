using GreenPantry.Domain.Entities;

namespace GreenPantry.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<Order>> GetByRestaurantIdAsync(Guid restaurantId);
    Task<Order> CreateAsync(Order order);
    Task<Order> UpdateAsync(Order order);
    Task<bool> DeleteAsync(Guid id);
}
