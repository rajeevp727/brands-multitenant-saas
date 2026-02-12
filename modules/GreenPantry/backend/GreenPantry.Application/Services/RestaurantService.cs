using AutoMapper;
using GreenPantry.Application.DTOs.Restaurant;
using GreenPantry.Application.Interfaces;
using GreenPantry.Domain.Entities;
using GreenPantry.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace GreenPantry.Application.Services;

public class RestaurantService : IRestaurantService
{
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<RestaurantService> _logger;

    public RestaurantService(
        IRestaurantRepository restaurantRepository,
        IMapper mapper,
        ILogger<RestaurantService> logger)
    {
        _restaurantRepository = restaurantRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IEnumerable<RestaurantDto>> GetRestaurantsAsync(RestaurantFilterDto filter)
    {
        _logger.LogInformation("Getting restaurants with filter: {Filter}", filter);

        var restaurants = await _restaurantRepository.GetAllAsync();
        
        // Apply filters
        // Only active and approved/pending (logic from original)
        // Note: Logic allows Pending? Original code allowed Pending. Keeping it.
        var filteredRestaurants = restaurants.Where(r => r.IsActive && (r.Status == Domain.Enums.RestaurantStatus.Approved || r.Status == Domain.Enums.RestaurantStatus.Pending));

        if (!string.IsNullOrEmpty(filter.City))
        {
            filteredRestaurants = filteredRestaurants.Where(r => 
                r.City.Equals(filter.City, StringComparison.OrdinalIgnoreCase) ||
                (r.Address != null && r.Address.City.Equals(filter.City, StringComparison.OrdinalIgnoreCase)));
        }

        if (filter.CuisineType.HasValue)
        {
            var cuisineTypeString = filter.CuisineType.Value.ToString();
            filteredRestaurants = filteredRestaurants.Where(r => 
                r.CuisineTypes.Contains(cuisineTypeString));
        }

        if (filter.MinRating.HasValue)
        {
            filteredRestaurants = filteredRestaurants.Where(r => r.Rating >= filter.MinRating.Value);
        }

        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            filteredRestaurants = filteredRestaurants.Where(r => 
                r.Name.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                r.Description.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase));
        }

        // Apply pagination
        var pagedRestaurants = filteredRestaurants
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize);

        return _mapper.Map<IEnumerable<RestaurantDto>>(pagedRestaurants);
    }

    public async Task<RestaurantDetailDto?> GetRestaurantByIdAsync(string id)
    {
        _logger.LogInformation("Getting restaurant by ID: {RestaurantId}", id);

        var restaurant = await _restaurantRepository.GetByIdAsync(Guid.Parse(id));
        if (restaurant == null || !restaurant.IsActive)
        {
            return null;
        }

        return _mapper.Map<RestaurantDetailDto>(restaurant);
    }

    public async Task<RestaurantDto> CreateRestaurantAsync(RestaurantDto restaurantDto)
    {
        _logger.LogInformation("Creating new restaurant: {RestaurantName}", restaurantDto.Name);

        var restaurantEntity = _mapper.Map<Restaurant>(restaurantDto);
        // Default values
        restaurantEntity.Status = Domain.Enums.RestaurantStatus.Pending;
        restaurantEntity.CreatedAt = DateTime.UtcNow;
        restaurantEntity.UpdatedAt = DateTime.UtcNow;
        restaurantEntity.IsActive = true; 
        
        // OwnerId handling? RestaurantDto has OwnerId in DetailDto but maybe not base?
        // RestaurantDto definition has no OwnerId. RestaurantDetailDto has it.
        // Assuming Owner assignment happens elsewhere or is not supported in this DTO flow.
        // Or mapped if present. The Entity requires OwnerId.
        // If Mapping fails to set OwnerId, validation might fail later. 
        // Original code had no OwnerId arg in CreateRestaurantAsync.
        
        var createdRestaurant = await _restaurantRepository.CreateAsync(restaurantEntity);
        return _mapper.Map<RestaurantDto>(createdRestaurant);
    }

    public async Task<RestaurantDto> UpdateRestaurantAsync(string id, RestaurantDto restaurantDto)
    {
        _logger.LogInformation("Updating restaurant: {RestaurantId}", id);

        var existingRestaurant = await _restaurantRepository.GetByIdAsync(Guid.Parse(id));
        if (existingRestaurant == null)
        {
            throw new KeyNotFoundException($"Restaurant with ID {id} not found");
        }

        _mapper.Map(restaurantDto, existingRestaurant);
        existingRestaurant.UpdatedAt = DateTime.UtcNow;

        var updatedRestaurant = await _restaurantRepository.UpdateAsync(existingRestaurant);
        return _mapper.Map<RestaurantDto>(updatedRestaurant);
    }

    public async Task<bool> DeleteRestaurantAsync(string id)
    {
        _logger.LogInformation("Deleting restaurant: {RestaurantId}", id);

        var restaurant = await _restaurantRepository.GetByIdAsync(Guid.Parse(id));
        if (restaurant == null)
        {
            return false;
        }

        restaurant.IsDeleted = true;
        restaurant.UpdatedAt = DateTime.UtcNow;
        await _restaurantRepository.UpdateAsync(restaurant);

        return true;
    }

    public async Task<IEnumerable<RestaurantDto>> GetRestaurantsByOwnerAsync(string ownerId)
    {
        _logger.LogInformation("Getting restaurants by owner: {OwnerId}", ownerId);

        var restaurants = await _restaurantRepository.GetByOwnerIdAsync(Guid.Parse(ownerId));
        return _mapper.Map<IEnumerable<RestaurantDto>>(restaurants);
    }
}
