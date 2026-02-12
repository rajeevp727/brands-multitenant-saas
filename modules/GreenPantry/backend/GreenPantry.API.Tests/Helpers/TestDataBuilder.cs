using AutoFixture;
using GreenPantry.Domain.Entities;

namespace GreenPantry.API.Tests.Helpers;

public class TestDataBuilder
{
    private readonly Fixture _fixture;

    public TestDataBuilder()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public Restaurant CreateRestaurant()
    {
        return _fixture.Build<Restaurant>()
            .Create();
    }

    public MenuItem CreateMenuItem(string? restaurantId = null)
    {
        return _fixture.Build<MenuItem>()
            .With(m => m.RestaurantId, restaurantId ?? _fixture.Create<string>())
            .Create();
    }

    public T Create<T>() => _fixture.Create<T>();
    public IEnumerable<T> CreateMany<T>(int count = 3) => _fixture.CreateMany<T>(count);
}
