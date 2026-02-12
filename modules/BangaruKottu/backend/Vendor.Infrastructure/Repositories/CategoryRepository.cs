using Vendor.Application.Interfaces;
using Vendor.Domain.Entities;
using Vendor.Infrastructure.Data;

namespace Vendor.Infrastructure.Repositories;

public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(ApplicationDbContext context) : base(context)
    {
    }
}

