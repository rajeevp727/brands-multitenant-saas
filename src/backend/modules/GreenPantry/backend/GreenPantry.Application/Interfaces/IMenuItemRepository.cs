using GreenPantry.Domain.Entities;

namespace GreenPantry.Application.Interfaces;

public interface IMenuItemRepository
{
    Task<MenuItem?> GetByIdAsync(Guid id);
    Task<IEnumerable<MenuItem>> GetByRestaurantIdAsync(Guid restaurantId);
    Task<MenuItem> CreateAsync(MenuItem menuItem);
    Task<MenuItem> UpdateAsync(MenuItem menuItem);
    Task<bool> DeleteAsync(Guid id);
}
