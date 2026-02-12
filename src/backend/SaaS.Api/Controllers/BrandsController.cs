using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using SaaS.Application.DTOs;
using SaaS.Application.Common;
using SaaS.Domain.Entities;
using SaaS.Infrastructure.Persistence;

namespace SaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IGenericService<BrandDto, Brand> _brandService;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public BrandsController(IGenericService<BrandDto, Brand> brandService, ApplicationDbContext context, IMapper mapper)
    {
        _brandService = brandService;
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllBrands()
    {
        // Bypass tenant filtering to get all brands from all tenants
        var brands = await _context.Brands
            .IgnoreQueryFilters()
            .Where(b => b.IsVisible)
            .ToListAsync();
        
        var brandDtos = _mapper.Map<List<BrandDto>>(brands);
        
        // Extract metadata from ConfigJson for each brand
        foreach (var brandDto in brandDtos)
        {
            MapConfigToDto(brandDto);
        }
        
        return Ok(brandDtos);
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentBrand()
    {
        // Global query filter handles TenantId
        var brands = await _brandService.GetAllAsync();
        var brand = brands.FirstOrDefault();
        
        if (brand == null) return NotFound("Brand configuration not found for this tenant.");
        
        MapConfigToDto(brand);
        return Ok(brand);
    }

    private void MapConfigToDto(BrandDto brandDto)
    {
        try
        {
            if (string.IsNullOrEmpty(brandDto.ConfigJson)) return;

            var config = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(brandDto.ConfigJson);
            if (config != null)
            {
                if (config.ContainsKey("port")) brandDto.Port = config["port"].GetString();
                if (config.ContainsKey("url")) brandDto.PortalUrl = config["url"].GetString() ?? string.Empty;
                
                // Fallback for fields if they are missing in the main columns but present in JSON
                if (string.IsNullOrEmpty(brandDto.Email) && config.ContainsKey("email")) brandDto.Email = config["email"].GetString() ?? string.Empty;
                if (string.IsNullOrEmpty(brandDto.Phone) && config.ContainsKey("phone")) brandDto.Phone = config["phone"].GetString() ?? string.Empty;
            }
        }
        catch
        {
            // If parsing fails, fields remain default
        }
    }
}
