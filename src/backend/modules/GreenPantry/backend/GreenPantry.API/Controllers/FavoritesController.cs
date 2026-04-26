using System.Collections.Generic;
using System.Threading.Tasks;
using GreenPantry.Application.DTOs.Favorite;
using GreenPantry.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GreenPantry.API.Controllers;

public class FavoritesController : BaseApiController
{
    private readonly IFavoriteService _favoriteService;

    public FavoritesController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FavoriteItemDto>>> GetMyFavorites()
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var favorites = await _favoriteService.GetUserFavoritesAsync(CurrentUserId);
        return Ok(favorites);
    }

    [HttpPost]
    public async Task<ActionResult<FavoriteDto>> AddFavorite(FavoriteAddDto dto)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var result = await _favoriteService.AddFavoriteAsync(CurrentUserId, dto);
        return Ok(result);
    }

    [HttpDelete("{itemType}/{itemId}")]
    public async Task<ActionResult> RemoveFavorite(string itemType, string itemId)
    {
        if (string.IsNullOrEmpty(CurrentUserId)) return Unauthorized();

        var success = await _favoriteService.RemoveFavoriteByItemIdAsync(CurrentUserId, itemId, itemType);
        
        if (!success)
            return NotFound();

        return NoContent();
    }
}
