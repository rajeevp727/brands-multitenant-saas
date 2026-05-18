using Mapster;
using Vendor.Application.DTOs.Category;
using Vendor.Application.Interfaces;

namespace Vendor.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.Adapt<IEnumerable<CategoryDto>>();
    }
}

