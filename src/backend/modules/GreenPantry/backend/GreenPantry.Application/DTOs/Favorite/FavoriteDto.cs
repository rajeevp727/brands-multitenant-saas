using System;

namespace GreenPantry.Application.DTOs.Favorite;

public class FavoriteDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
}

public class FavoriteAddDto
{
    public string ItemId { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
}
