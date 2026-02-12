using GreenPantry.Domain.Entities;

namespace GreenPantry.Application.Interfaces;

public interface IRestaurantRepository
{
    Task<Restaurant?> GetByIdAsync(Guid id);
    Task<IEnumerable<Restaurant>> GetAllAsync();
    Task<IEnumerable<Restaurant>> GetByOwnerIdAsync(Guid ownerId);
    Task<Restaurant> CreateAsync(Restaurant restaurant);
    Task<Restaurant> UpdateAsync(Restaurant restaurant);
    Task<bool> DeleteAsync(Guid id);
}
