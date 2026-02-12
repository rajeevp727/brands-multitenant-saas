using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SaaS.Api.Controllers;
using SaaS.Application.Common;
using SaaS.Application.DTOs;
using SaaS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Api.Tests.Controllers
{
    public class UnifiedControllerTests
    {
        private readonly Mock<IServiceProvider> _mockServiceProvider;
        private readonly Mock<IGenericService<ProductDto, Product>> _mockProductService;
        private readonly UnifiedController _controller;

        public UnifiedControllerTests()
        {
            _mockServiceProvider = new Mock<IServiceProvider>();
            _mockProductService = new Mock<IGenericService<ProductDto, Product>>();
            
            // Setup ServiceProvider to return our mock service
            _mockServiceProvider
                .Setup(x => x.GetService(typeof(IGenericService<ProductDto, Product>)))
                .Returns(_mockProductService.Object);

            _controller = new UnifiedController(_mockServiceProvider.Object);
        }

        [Fact]
        public async Task GetAll_ValidResource_ReturnsOk()
        {
            // Arrange
            var products = new List<ProductDto> { new ProductDto { Name = "P1" } };
            _mockProductService.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetAll("products");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(products);
        }

        [Fact]
        public async Task GetById_ValidResourceAndId_ReturnsOk()
        {
            // Arrange
            var product = new ProductDto { Name = "P1" };
            _mockProductService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(product);

            // Act
            var result = await _controller.GetById("products", "1");

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(product);
        }

        [Fact]
        public async Task GetAll_InvalidResource_ReturnsNotFound()
        {
            // Act
            var result = await _controller.GetAll("invalid");

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task Create_ValidResource_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new { Name = "New Product" };
            var createdDto = new ProductDto { Name = "New Product" };
            _mockProductService.Setup(s => s.CreateAsync(It.IsAny<ProductDto>())).ReturnsAsync(createdDto);

            // Act
            var result = await _controller.Create("products", dto);

            // Assert
            result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task Update_ValidResource_ReturnsNoContent()
        {
            // Arrange
            var dto = new { Name = "Updated Product" };
            _mockProductService.Setup(s => s.UpdateAsync("1", It.IsAny<ProductDto>())).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Update("products", "1", dto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ValidResource_ReturnsNoContent()
        {
            // Arrange
            _mockProductService.Setup(s => s.DeleteAsync("1")).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete("products", "1");

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task GetAll_ServiceNotFound_ReturnsInternalServerError()
        {
            // Arrange
            _mockServiceProvider.Setup(x => x.GetService(It.IsAny<Type>())).Returns(null);

            // Act
            var result = await _controller.GetAll("products");

            // Assert
            result.Should().BeOfType<ObjectResult>();
            (result as ObjectResult).StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task GetById_ServiceThrows_ReturnsInternalServerError()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetByIdAsync("1")).ThrowsAsync(new Exception("error"));

            // Act
            var result = await _controller.GetById("products", "1");

            // Assert
            result.Should().BeOfType<ObjectResult>();
            (result as ObjectResult).StatusCode.Should().Be(500);
        }
    }
}
