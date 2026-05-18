using Mapster;
using SaaS.Application.DTOs;
using SaaS.Domain.Entities;

namespace SaaS.Application.Mappings;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Product, ProductDto>.NewConfig().TwoWays();
        TypeAdapterConfig<User, UserDto>.NewConfig().TwoWays();
        TypeAdapterConfig<Order, OrderDto>.NewConfig().TwoWays();
        TypeAdapterConfig<Brand, BrandDto>.NewConfig().TwoWays();
    }
}
