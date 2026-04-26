using System.Collections.Generic;
using System.Threading.Tasks;
using GreenPantry.Application.DTOs.Favorite;

namespace GreenPantry.Application.Interfaces;

public interface IFavoriteService
{
    Task<FavoriteDto> AddFavoriteAsync(string userId, FavoriteAddDto dto);
    Task<bool> RemoveFavoriteByItemIdAsync(string userId, string itemId, string itemType);
    Task<IEnumerable<FavoriteItemDto>> GetUserFavoritesAsync(string userId);
}
