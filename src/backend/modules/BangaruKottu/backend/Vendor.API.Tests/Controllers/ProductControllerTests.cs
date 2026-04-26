using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Vendor.API.Controllers;
using Vendor.API.Tests.Helpers;
using Vendor.Application.DTOs.Product;
using Vendor.Application.Interfaces;
using Xunit;

namespace Vendor.API.Tests.Controllers;

public class ProductControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<ILogger<ProductController>> _mockLogger;
    private readonly ProductController _controller;
    private readonly TestDataBuilder _dataBuilder;
    private const int VendorId = 101;

    public ProductControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _mockLogger = new Mock<ILogger<ProductController>>();
        _controller = new ProductController(_mockProductService.Object, _mockLogger.Object);
        _dataBuilder = new TestDataBuilder();

        // Setup HttpContext with Claims
        var claims = new[] { new Claim("VendorId", VendorId.ToString()), new Claim(ClaimTypes.Role, "Vendor") };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task GetProducts_ReturnsOk_WithVendorProducts()
    {
        // Arrange
        var products = _dataBuilder.CreateMany<ProductDto>(2);
        _mockProductService.Setup(s => s.GetVendorProductsAsync(VendorId)).ReturnsAsync(products);

        // Act
        var result = await _controller.GetProducts();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(products);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = _dataBuilder.Create<CreateProductRequest>();
        var productDto = _dataBuilder.Create<ProductDto>();
        _mockProductService.Setup(s => s.CreateProductAsync(VendorId, request)).ReturnsAsync(productDto);

        // Act
        var result = await _controller.CreateProduct(request);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsNoContent()
    {
        // Arrange
        var productId = 1;
        _mockProductService.Setup(s => s.DeleteProductAsync(productId, VendorId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteProduct(productId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }
}
