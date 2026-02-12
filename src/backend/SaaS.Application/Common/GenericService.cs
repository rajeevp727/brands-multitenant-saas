using AutoMapper;
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
    protected readonly IMapper _mapper;

    public GenericService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TDto>> GetAllAsync()
    {
        var entities = await _unitOfWork.Repository<TEntity>().GetAllAsync();
        return _mapper.Map<IEnumerable<TDto>>(entities);
    }

    public async Task<TDto?> GetByIdAsync(object id)
    {
        var entity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id);
        return _mapper.Map<TDto>(entity);
    }

    public async Task<TDto> CreateAsync(TDto dto)
    {
        var entity = _mapper.Map<TEntity>(dto);
        await _unitOfWork.Repository<TEntity>().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TDto>(entity);
    }

    public async Task UpdateAsync(object id, TDto dto)
    {
        var entity = await _unitOfWork.Repository<TEntity>().GetByIdAsync(id);
        if (entity == null) throw new Exception("Entity not found");

        _mapper.Map(dto, entity);
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
