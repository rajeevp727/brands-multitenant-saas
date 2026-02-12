namespace SaaS.Domain.Interfaces;

public interface ITenantProvider
{
    string? GetTenantId();
}
