using Mapster;
using GreenPantry.Application.DTOs.Auth;
using GreenPantry.Application.DTOs.Menu;
using GreenPantry.Application.DTOs.Order;
using GreenPantry.Application.DTOs.Restaurant;
using GreenPantry.Domain.Entities;

namespace GreenPantry.Application.Mappings;

public static class MapsterConfig
{
    public static void Configure()
    {
        TypeAdapterConfig<Restaurant, RestaurantDto>
            .NewConfig()
            .Map(dest => dest.Address, src => src.Address != null ? $"{src.Address.Street}, {src.Address.City}, {src.Address.State} {src.Address.PostalCode}" : string.Empty);

        TypeAdapterConfig<Restaurant, RestaurantDetailDto>
            .NewConfig()
            .Map(dest => dest.Address, src => src.Address != null ? $"{src.Address.Street}, {src.Address.City}, {src.Address.State} {src.Address.PostalCode}" : string.Empty);

        TypeAdapterConfig<RestaurantDto, Restaurant>
            .NewConfig()
            .Ignore(dest => dest.Address!);

        TypeAdapterConfig<RestaurantDetailDto, Restaurant>
            .NewConfig()
            .Ignore(dest => dest.Address!);

        TypeAdapterConfig<CreateOrderRequest, Order>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt!)
            .Ignore(dest => dest.UpdatedAt!)
            .Ignore(dest => dest.IsDeleted)
            .Ignore(dest => dest.OrderNumber)
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.Items)
            .Ignore(dest => dest.SubTotal)
            .Ignore(dest => dest.DeliveryFee)
            .Ignore(dest => dest.Tax)
            .Ignore(dest => dest.Total)
            .Ignore(dest => dest.PaymentId)
            .Ignore(dest => dest.EstimatedDeliveryTime!)
            .Ignore(dest => dest.DeliveredAt!)
            .Ignore(dest => dest.DeliveryPersonId)
            .Ignore(dest => dest.StatusHistory);
    }
}
