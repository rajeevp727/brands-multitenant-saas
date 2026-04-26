using System;

namespace GreenPantry.Application.DTOs.Favorite;

public class FavoriteItemDto
{
    public string Id { get; set; } = string.Empty; // Favorite Record ID
    public string ItemId { get; set; } = string.Empty; // Restaurant/Dish ID
    public string Type { get; set; } = string.Empty; // "restaurant" or "dish"
    
    // Shared
    public string Name { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public DateTime AddedDate { get; set; }

    // Restaurant specific
    public string Cuisine { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string DeliveryTime { get; set; } = string.Empty;
    public string Distance { get; set; } = string.Empty;
    public string PriceRange { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsOpen { get; set; }
    public decimal DeliveryFee { get; set; }

    // Dish specific
    public string Category { get; set; } = string.Empty;
    public string Restaurant { get; set; } = string.Empty; // Name of dish's restaurant
    public decimal Price { get; set; }
}
