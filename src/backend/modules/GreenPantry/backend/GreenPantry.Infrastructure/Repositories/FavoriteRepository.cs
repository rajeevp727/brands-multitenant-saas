using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GreenPantry.Application.Interfaces;
using GreenPantry.Domain.Entities;
using GreenPantry.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GreenPantry.Infrastructure.Repositories;

public class FavoriteRepository : IFavoriteRepository
{
    private readonly AppDbContext _context;

    public FavoriteRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Favorite> GetFavoriteAsync(string userId, string itemId, string itemType)
    {
        return await _context.Favorites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.ItemId == itemId && f.ItemType == itemType);
    }

    public async Task<IEnumerable<Favorite>> GetUserFavoritesAsync(string userId)
    {
        return await _context.Favorites
            .Where(f => f.UserId == userId)
            .ToListAsync();
    }

    public async Task<Favorite> CreateAsync(Favorite favorite)
    {
        await _context.Favorites.AddAsync(favorite);
        await _context.SaveChangesAsync();
        return favorite;
    }

    public async Task DeleteAsync(Favorite favorite)
    {
        _context.Favorites.Remove(favorite);
        await _context.SaveChangesAsync();
    }
}
