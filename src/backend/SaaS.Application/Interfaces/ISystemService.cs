namespace SaaS.Application.Interfaces;

public interface ISystemService
{
    Task<bool> IsDeploymentActiveAsync();
    Task SetDeploymentStatusAsync(bool isActive);
}
