using Microsoft.AspNetCore.Mvc;
using SaaS.Domain.Entities;
using SaaS.Domain.Interfaces;

namespace SaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConstantsController : ControllerBase
{
    private readonly IGenericRepository<AppConstant> _constantsRepository;
    private readonly ITenantProvider _tenantProvider;

    public ConstantsController(IGenericRepository<AppConstant> constantsRepository, ITenantProvider tenantProvider)
    {
        _constantsRepository = constantsRepository;
        _tenantProvider = tenantProvider;
    }

    [HttpGet]
    public async Task<IActionResult> GetConstants()
    {
        var tenantId = _tenantProvider.GetTenantId();
        var allConstants = await _constantsRepository.GetAllAsync();
        
        // Create dictionary with global defaults, then override with tenant-specific
        var result = new Dictionary<string, string>();
        
        foreach (var constant in allConstants.Where(c => c.TenantId == null))
        {
            result[constant.ConstantKey] = constant.ConstantValue;
        }
        
        foreach (var constant in allConstants.Where(c => c.TenantId == tenantId))
        {
            result[constant.ConstantKey] = constant.ConstantValue;
        }
        
        return Ok(result);
    }
}
