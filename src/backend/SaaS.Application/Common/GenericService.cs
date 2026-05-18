using Mapster;
using SaaS.Domain.Interfaces;

namespace SaaS.Application.Common;

public interface IGenericService<TDto, TEntity> where TEntity : class
{
    Task<IEnumerable<TDto>> GetAllAsync();
    Task<TDto?> GetByIdAsync(object id);
    Task<TDto> CreateAsync(TDto dto);
    Task UpdateAsync(object id, TDto dto);
    Task DeleteAsync(object id);
}

public class GenericService<TDto, TEntity> : IGenericService<TDto, TEntity> where TEntity : class
{
    protected readonly IUnitOfWork _unitOfWork;

    public GenericService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TDto>> GetAllAsync()
    {
        var entities = await _unitOfWork.Repository<TEntity>().GetAllAsync();
        return entities.Adapt<IEnumerable<TDto>>();
    }

    public async Task<TDto?> GetByIdAsync(object id)
    {
        var entity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id);
        return entity.Adapt<TDto>();
    }

    public async Task<TDto> CreateAsync(TDto dto)
    {
        var entity = dto.Adapt<TEntity>();
        await _unitOfWork.Repository<TEntity>().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.Adapt<TDto>();
    }

    public async Task UpdateAsync(object id, TDto dto)
    {
        var entity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id);
        if (entity == null) throw new Exception("Entity not found");

        dto.Adapt(entity);
        _unitOfWork.Repository<TEntity>().Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(object id)
    {
        var entity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id);
        if (entity == null) throw new Exception("Entity not found");

        _unitOfWork.Repository<TEntity>().Delete(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}
