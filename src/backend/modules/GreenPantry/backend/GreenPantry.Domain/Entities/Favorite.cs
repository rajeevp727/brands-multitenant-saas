using System;

namespace GreenPantry.Domain.Entities;

public class Favorite : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty; // "Restaurant" or "Dish"
}
