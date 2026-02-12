using AutoMapper;
using Vendor.Application.DTOs.Category;
using Vendor.Application.DTOs.Order;
using Vendor.Application.DTOs.Product;
using Vendor.Application.DTOs.Vendor;
using Vendor.Domain.Entities;

namespace Vendor.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<VendorEntity, VendorProfileDto>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User != null ? src.User.Email : string.Empty));

        CreateMap<Category, CategoryDto>();
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.CategoryName : string.Empty));

        CreateMap<Order, OrderDto>();
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.ProductName : string.Empty));
    }
}

