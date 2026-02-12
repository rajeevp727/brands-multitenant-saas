using AutoFixture;
using SaaS.Domain.Entities;

namespace SaaS.Api.Tests.Helpers;

public class TestDataBuilder
{
    private readonly Fixture _fixture;

    public TestDataBuilder()
    {
        _fixture = new Fixture();
        // Prevent circular references
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
    }

    public User CreateUser(string? email = null)
    {
        return _fixture.Build<User>()
            .With(u => u.Email, email ?? _fixture.Create<string>() + "@example.com")
            .Without(u => u.Role)
            .Create();
    }

    public Brand CreateBrand()
    {
        return _fixture.Build<Brand>()
            .Create();
    }

    public T Create<T>() => _fixture.Create<T>();
    public IEnumerable<T> CreateMany<T>(int count = 3) => _fixture.CreateMany<T>(count);
}
