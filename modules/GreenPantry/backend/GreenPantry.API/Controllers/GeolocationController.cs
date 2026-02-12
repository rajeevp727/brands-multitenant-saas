using GreenPantry.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using GreenPantry.Application.DTOs.Geolocation;

namespace GreenPantry.API.Controllers;

public class GeolocationController : BaseApiController
{
    private readonly IGeolocationService _geolocationService;

    public GeolocationController(IGeolocationService geolocationService)
    {
        _geolocationService = geolocationService;
    }

    [HttpGet("address")]
    public async Task<ActionResult<GeolocationResponse>> GetLocationFromCoordinates(double latitude, double longitude)
    {
        var location = await _geolocationService.GetLocationFromCoordinatesAsync(latitude, longitude);
        return Ok(location);
    }

    [HttpGet("ip")]
    public async Task<ActionResult<GeolocationResponse>> GetLocationFromIP()
    {
        var ipAddress = Request.HttpContext.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
        {
            // For local development, use a sample IP
            ipAddress = "8.8.8.8"; 
        }
        
        var location = await _geolocationService.GetLocationFromIPAsync(ipAddress);
        return Ok(location);
    }
}
