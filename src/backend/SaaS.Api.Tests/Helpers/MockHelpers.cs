using Microsoft.Extensions.Logging;
using Moq;

namespace SaaS.Api.Tests.Helpers;

public static class MockHelpers
{
    public static Mock<ILogger<T>> MockLogger<T>()
    {
        return new Mock<ILogger<T>>();
    }

    // Add more shared mocking helpers here (e.g., HttpContext, Identity)
}
