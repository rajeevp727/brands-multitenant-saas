using AutoMapper;
using GreenPantry.Application.DTOs.Auth;
using GreenPantry.Application.DTOs.Order;
using GreenPantry.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace GreenPantry.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UserService> _logger;

    public UserService(
        IUserRepository userRepository,
        IOrderRepository orderRepository,
        IMapper mapper,
        ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UserDto?> GetUserByIdAsync(string id)
    {
        _logger.LogInformation("Getting user by ID: {UserId}", id);

        var user = await _userRepository.GetByIdAsync(Guid.Parse(id));
        if (user == null || user.IsDeleted)
        {
            return null;
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> UpdateUserProfileAsync(string id, UserDto user)
    {
        _logger.LogInformation("Updating user profile: {UserId}", id);

        var existingUser = await _userRepository.GetByIdAsync(Guid.Parse(id));
        if (existingUser == null)
        {
            throw new KeyNotFoundException($"User with ID {id} not found");
        }

        // Update allowed fields
        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.PhoneNumber = user.PhoneNumber;
        existingUser.StreetAddress = user.StreetAddress;
        existingUser.City = user.City;
        existingUser.State = user.State;
        existingUser.PostalCode = user.PostalCode;
        existingUser.Country = user.Country;
        existingUser.Latitude = user.Latitude;
        existingUser.Longitude = user.Longitude;
        existingUser.UpdatedAt = DateTime.UtcNow;

        var updatedUser = await _userRepository.UpdateAsync(existingUser);
        return _mapper.Map<UserDto>(updatedUser);
    }

    public async Task<bool> UpdateUserAddressAsync(string id, GreenPantry.Application.DTOs.Auth.AddressDto address)
    {
        _logger.LogInformation("Updating user address: {UserId}", id);

        var user = await _userRepository.GetByIdAsync(Guid.Parse(id));
        if (user == null)
        {
            return false;
        }

        user.StreetAddress = address.Street;
        user.City = address.City;
        user.State = address.State;
        user.PostalCode = address.PostalCode;
        user.Country = address.Country;
        user.Latitude = address.Latitude;
        user.Longitude = address.Longitude;
        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        return true;
    }

    public async Task<IEnumerable<OrderDto>> GetUserOrderHistoryAsync(string userId)
    {
        _logger.LogInformation("Getting order history for user: {UserId}", userId);

        var orders = await _orderRepository.GetByUserIdAsync(Guid.Parse(userId));
        var activeOrders = orders.Where(o => !o.IsDeleted).OrderByDescending(o => o.CreatedAt);

        return _mapper.Map<IEnumerable<OrderDto>>(activeOrders);
    }
}
