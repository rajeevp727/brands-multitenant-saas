using SaaS.Domain.Interfaces;

namespace SaaS.Infrastructure.Services;

public class UserContext : IUserContext
{
    public string? UserId => "System"; // TODO: Implement proper user context from authentication
}