using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPantry.Application.DTOs.Favorite;
using GreenPantry.Application.Interfaces;
using GreenPantry.Domain.Entities;

namespace GreenPantry.Application.Services;

public class FavoriteService : IFavoriteService
{
    private readonly IFavoriteRepository _favoriteRepository;
    private readonly IRestaurantRepository _restaurantRepository;
    private readonly IMenuItemRepository _menuItemRepository;

    public FavoriteService(
        IFavoriteRepository favoriteRepository,
        IRestaurantRepository restaurantRepository,
        IMenuItemRepository menuItemRepository)
    {
        _favoriteRepository = favoriteRepository;
        _restaurantRepository = restaurantRepository;
        _menuItemRepository = menuItemRepository;
    }

    public async Task<FavoriteDto> AddFavoriteAsync(string userId, FavoriteAddDto dto)
    {
        var existing = await _favoriteRepository.GetFavoriteAsync(userId, dto.ItemId, dto.ItemType);

        if (existing != null)
        {
            return new FavoriteDto
            {
                Id = existing.Id.ToString(),
                UserId = existing.UserId,
                ItemId = existing.ItemId,
                ItemType = existing.ItemType
            };
        }

        var favorite = new Favorite
        {
            UserId = userId,
            ItemId = dto.ItemId,
            ItemType = dto.ItemType,
            CreatedBy = userId
        };

        var saved = await _favoriteRepository.CreateAsync(favorite);

        return new FavoriteDto
        {
            Id = saved.Id.ToString(),
            UserId = saved.UserId,
            ItemId = saved.ItemId,
            ItemType = saved.ItemType
        };
    }

    public async Task<bool> RemoveFavoriteByItemIdAsync(string userId, string itemId, string itemType)
    {
        var favorite = await _favoriteRepository.GetFavoriteAsync(userId, itemId, itemType);

        if (favorite == null) return false;

        await _favoriteRepository.DeleteAsync(favorite);
        return true;
    }

    public async Task<IEnumerable<FavoriteItemDto>> GetUserFavoritesAsync(string userId)
    {
        var favorites = await _favoriteRepository.GetUserFavoritesAsync(userId);
        var result = new List<FavoriteItemDto>();

        foreach (var fav in favorites)
        {
            if (fav.ItemType.ToLower() == "restaurant")
            {
                var r = await _restaurantRepository.GetByIdAsync(Guid.Parse(fav.ItemId));
                if (r != null)
                {
                    result.Add(new FavoriteItemDto
                    {
                        Id = fav.Id.ToString(),
                        ItemId = r.Id.ToString(),
                        Type = "restaurant",
                        Name = r.Name,
                        Cuisine = r.CuisineTypes?.FirstOrDefault() ?? "Restaurant",
                        Rating = r.Rating,
                        DeliveryTime = r.EstimatedDeliveryTime + " min",
                        Distance = "2.5 km",
                        Image = r.ImageUrl,
                        PriceRange = "₹₹",
                        Address = r.Address != null ? $"{r.Address.Street}, {r.Address.City}" : "",
                        IsOpen = r.IsActive,
                        DeliveryFee = r.DeliveryFee,
                        AddedDate = fav.CreatedAt
                    });
                }
            }
            else if (fav.ItemType.ToLower() == "dish")
            {
                var d = await _menuItemRepository.GetByIdAsync(Guid.Parse(fav.ItemId));
                if (d != null)
                {
                    var r = await _restaurantRepository.GetByIdAsync(d.RestaurantId);
                    result.Add(new FavoriteItemDto
                    {
                        Id = fav.Id.ToString(),
                        ItemId = d.Id.ToString(),
                        Type = "dish",
                        Name = d.Name,
                        Restaurant = r != null ? r.Name : "Restaurant",
                        Price = d.Price,
                        Image = d.ImageUrl,
                        Category = d.Category,
                        AddedDate = fav.CreatedAt
                    });
                }
            }
        }

        return result.OrderByDescending(x => x.AddedDate).ToList();
    }
}
