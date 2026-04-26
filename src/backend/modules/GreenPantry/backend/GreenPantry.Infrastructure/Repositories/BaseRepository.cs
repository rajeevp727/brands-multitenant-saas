using GreenPantry.Infrastructure.Data;
using GreenPantry.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GreenPantry.Infrastructure.Repositories;

public abstract class BaseRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger _logger;

    protected BaseRepository(AppDbContext context, ILogger logger)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _logger = logger;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        try
        {
            return await _dbSet.FindAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting entity by ID: {Id}", id);
            throw;
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        try
        {
            return await _dbSet.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all entities");
            throw;
        }
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        try
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating entity");
            throw;
        }
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        try
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating entity");
            throw;
        }
    }

    public virtual async Task<bool> DeleteAsync(Guid id)
    {
        try
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting entity with ID: {Id}", id);
            throw;
        }
    }
}
