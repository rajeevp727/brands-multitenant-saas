using Microsoft.AspNetCore.Mvc;
using SaaS.Application.Common;

namespace SaaS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class GenericController<TDto, TEntity> : ControllerBase where TEntity : class
{
    protected readonly IGenericService<TDto, TEntity> _service;

    protected GenericController(IGenericService<TDto, TEntity> service)
    {
        _service = service;
    }

    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<TDto>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TDto>> GetById([FromRoute] string id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public virtual async Task<ActionResult<TDto>> Create([FromBody] TDto dto)
    {
        var result = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = "1" }, result); // Simplified ID
    }

    [HttpPut("{id}")]
    public virtual async Task<IActionResult> Update([FromRoute] string id, [FromBody] TDto dto)
    {
        await _service.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> Delete([FromRoute] string id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }
}
