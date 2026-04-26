using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GreenPantry.Domain.Entities;

namespace GreenPantry.Application.Interfaces;

public interface IFavoriteRepository
{
    Task<Favorite> GetFavoriteAsync(string userId, string itemId, string itemType);
    Task<IEnumerable<Favorite>> GetUserFavoritesAsync(string userId);
    Task<Favorite> CreateAsync(Favorite favorite);
    Task DeleteAsync(Favorite favorite);
}
