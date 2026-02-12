using SaaS.Domain.Entities;

namespace SaaS.Domain.Interfaces;

public interface IJwtProvider
{
    string GenerateToken(User user);
}
