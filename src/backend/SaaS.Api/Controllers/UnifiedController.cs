using Microsoft.AspNetCore.Mvc;
using SaaS.Application.Common;
using SaaS.Application.DTOs;
using SaaS.Domain.Entities;
using System.Reflection;

namespace SaaS.Api.Controllers;

[ApiController]
[Route("api/{resource}")]
public class UnifiedController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly Dictionary<string, (Type Dto, Type Entity)> _resourceMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "products", (typeof(ProductDto), typeof(Product)) },
        { "orders", (typeof(OrderDto), typeof(Order)) },
        { "users", (typeof(UserDto), typeof(User)) }
    };

    public UnifiedController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    private object ResolveService(string resource)
    {
        if (!_resourceMap.TryGetValue(resource, out var types))
            throw new KeyNotFoundException($"Resource '{resource}' not registered.");

        var serviceType = typeof(IGenericService<,>).MakeGenericType(types.Dto, types.Entity);
        var service = _serviceProvider.GetService(serviceType);
        
        if (service == null)
            throw new InvalidOperationException($"Service for '{resource}' not found.");
            
        return service;
    }

    private async Task<IActionResult> InvokeServiceMethod(string resource, string methodName, params object[] args)
    {
        try
        {
            var service = ResolveService(resource);
            var method = service.GetType().GetMethod(methodName);
            
            if (method == null)
                return BadRequest($"Method {methodName} not supported for {resource}");

            var task = (Task)method.Invoke(service, args)!;
            await task.ConfigureAwait(false);

            // Access "Result" property if it exists (for Task<T>)
            var resultProperty = task.GetType().GetProperty("Result");
            if (resultProperty != null)
            {
                var result = resultProperty.GetValue(task);
                if (result == null && methodName != "GetAllAsync") return NotFound(); 
                return Ok(result);
            }

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Resource not found");
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet]
    public Task<IActionResult> GetAll(string resource) => InvokeServiceMethod(resource, "GetAllAsync");

    [HttpGet("{id}")]
    public Task<IActionResult> GetById(string resource, string id) => InvokeServiceMethod(resource, "GetByIdAsync", id);

    [HttpPost]
    public async Task<IActionResult> Create(string resource, [FromBody] object dto)
    {
        // Must convert dynamic JObject/JsonElement to specific DTO type
        if (!_resourceMap.TryGetValue(resource, out var types)) return NotFound();

        try 
        {
            // Simple deserialization if using System.Text.Json (default in ASP.NET Core)
            var json = System.Text.Json.JsonSerializer.Serialize(dto);
            var typedDto = System.Text.Json.JsonSerializer.Deserialize(json, types.Dto);

            var service = ResolveService(resource);
            var method = service.GetType().GetMethod("CreateAsync")!;
            var task = (Task)method.Invoke(service, new[] { typedDto })!;
            await task.ConfigureAwait(false);
            
            var result = task.GetType().GetProperty("Result")?.GetValue(task);
            return CreatedAtAction(nameof(GetById), new { resource, id = "new" }, result);
        }
        catch (Exception ex)
        {
             return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string resource, string id, [FromBody] object dto)
    {
         if (!_resourceMap.TryGetValue(resource, out var types)) return NotFound();

        try 
        {
            var json = System.Text.Json.JsonSerializer.Serialize(dto);
            var typedDto = System.Text.Json.JsonSerializer.Deserialize(json, types.Dto);
            
            var service = ResolveService(resource);
            var method = service.GetType().GetMethod("UpdateAsync")!;
            await (Task)method.Invoke(service, new[] { id, typedDto })!;
            
            return NoContent();
        }
        catch (Exception ex)
        {
             return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string resource, string id)
    {
        try
        {
            var service = ResolveService(resource);
            var method = service.GetType().GetMethod("DeleteAsync")!;
            await (Task)method.Invoke(service, new[] { id })!;
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
