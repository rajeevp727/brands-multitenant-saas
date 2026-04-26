using AutoFixture;
using Vendor.Domain.Entities;

namespace Vendor.API.Tests.Helpers;

public class TestDataBuilder
{
    private readonly Fixture _fixture;

    public TestDataBuilder()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public Product CreateProduct(int vendorId)
    {
        return _fixture.Build<Product>()
            .With(p => p.VendorId, vendorId)
            .Without(p => p.Vendor)
            .Without(p => p.Category)
            .Create();
    }

    public T Create<T>() => _fixture.Create<T>();
    public IEnumerable<T> CreateMany<T>(int count = 3) => _fixture.CreateMany<T>(count);
}
