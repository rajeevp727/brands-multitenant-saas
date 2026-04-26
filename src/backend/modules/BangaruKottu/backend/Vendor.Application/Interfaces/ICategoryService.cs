using Vendor.Application.DTOs.Category;

namespace Vendor.Application.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
}

