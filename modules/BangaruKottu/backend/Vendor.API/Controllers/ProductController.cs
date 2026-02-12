using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vendor.API.Extensions;
using Vendor.Application.DTOs.Product;
using Vendor.Application.Interfaces;

namespace Vendor.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Vendor")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        var vendorId = User.GetVendorId();
        var products = await _productService.GetVendorProductsAsync(vendorId);
        return Ok(products);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var vendorId = User.GetVendorId();
        var product = await _productService.GetProductByIdAsync(id, vendorId);
        if (product == null)
            return NotFound(new { message = "Product not found" });

        return Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductRequest request)
    {
        var vendorId = User.GetVendorId();
        try
        {
            var product = await _productService.CreateProductAsync(vendorId, request);
            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, [FromBody] UpdateProductRequest request)
    {
        var vendorId = User.GetVendorId();
        try
        {
            var product = await _productService.UpdateProductAsync(id, vendorId, request);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            return Ok(product);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var vendorId = User.GetVendorId();
        var deleted = await _productService.DeleteProductAsync(id, vendorId);
        if (!deleted)
            return NotFound(new { message = "Product not found" });

        return NoContent();
    }

    [HttpPatch("{id}/toggle-status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ToggleProductStatus(int id)
    {
        var vendorId = User.GetVendorId();
        var toggled = await _productService.ToggleProductStatusAsync(id, vendorId);
        if (!toggled)
            return NotFound(new { message = "Product not found" });

        return Ok(new { message = "Product status updated" });
    }
}

