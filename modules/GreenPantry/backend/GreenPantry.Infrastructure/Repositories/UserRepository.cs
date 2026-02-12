using GreenPantry.Application.Interfaces;
using GreenPantry.Domain.Entities;
using GreenPantry.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GreenPantry.Infrastructure.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(AppDbContext context, ILogger<UserRepository> logger) 
        : base(context, logger)
    {
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        try
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && !u.IsDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by email: {Email}", email);
            throw;
        }
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        try
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && !u.IsDeleted);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user by refresh token");
            throw;
        }
    }

    public override async Task<User?> GetByIdAsync(Guid id)
    {
        return await base.GetByIdAsync(id);
    }

    public new async Task<IEnumerable<User>> GetAllAsync()
    {
        return await base.GetAllAsync();
    }

    public override async Task<User> CreateAsync(User user)
    {
        return await base.CreateAsync(user);
    }

    public override async Task<User> UpdateAsync(User user)
    {
        return await base.UpdateAsync(user);
    }

    public override async Task<bool> DeleteAsync(Guid id)
    {
        return await base.DeleteAsync(id);
    }
}
