using Mapster;
using Vendor.Application.DTOs.Category;
using Vendor.Application.DTOs.Order;
using Vendor.Application.DTOs.Product;
using Vendor.Application.DTOs.Vendor;
using Vendor.Domain.Entities;

namespace Vendor.Application.Mapping;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<VendorEntity, VendorProfileDto>
            .NewConfig()
            .Map(dest => dest.Email, src => src.User != null ? src.User.Email : string.Empty);

        TypeAdapterConfig<Category, CategoryDto>
            .NewConfig();

        TypeAdapterConfig<Product, ProductDto>
            .NewConfig()
            .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.CategoryName : string.Empty);

        TypeAdapterConfig<Order, OrderDto>
            .NewConfig();

        TypeAdapterConfig<OrderItem, OrderItemDto>
            .NewConfig()
            .Map(dest => dest.ProductName, src => src.Product != null ? src.Product.ProductName : string.Empty);
    }
}
