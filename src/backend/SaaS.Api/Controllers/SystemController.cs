using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SaaS.Application.Interfaces;
using System.Threading.Tasks;

namespace SaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SystemController : ControllerBase
{
    private readonly ISystemService _systemService;

    public SystemController(ISystemService systemService)
    {
        _systemService = systemService;
    }

    [HttpGet("status")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStatus()
    {
        var isDeploying = await _systemService.IsDeploymentActiveAsync();
        return Ok(new { isDeploying });
    }

    // Webhook for Vercel/Render
    // Note: In production, you'd want to verify a secret token in the header
    [HttpPost("webhook/deploy-start")]
    [AllowAnonymous]
    public async Task<IActionResult> OnDeployStart()
    {
        await _systemService.SetDeploymentStatusAsync(true);
        return Ok();
    }

    [HttpPost("webhook/deploy-success")]
    [AllowAnonymous]
    public async Task<IActionResult> OnDeploySuccess()
    {
        await _systemService.SetDeploymentStatusAsync(false);
        return Ok();
    }
}
