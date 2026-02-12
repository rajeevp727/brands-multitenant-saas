using AutoMapper;
using SaaS.Application.DTOs;
using SaaS.Domain.Entities;

namespace SaaS.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();
        CreateMap<User, UserDto>().ReverseMap();
        CreateMap<Order, OrderDto>().ReverseMap();
        CreateMap<Brand, BrandDto>().ReverseMap();
    }
}
