using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Vendor.API.Controllers;
using Vendor.Application.DTOs.Category;
using Vendor.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vendor.API.Tests.Controllers
{
    public class CategoryControllerTests
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly Mock<ILogger<CategoryController>> _mockLogger;
        private readonly CategoryController _controller;

        public CategoryControllerTests()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _mockLogger = new Mock<ILogger<CategoryController>>();
            _controller = new CategoryController(_mockCategoryService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetCategories_ReturnsOkWithList()
        {
            // Arrange
            var categories = new List<CategoryDto> { new CategoryDto { CategoryName = "C1" } };
            _mockCategoryService.Setup(s => s.GetAllCategoriesAsync()).ReturnsAsync(categories);

            // Act
            var result = await _controller.GetCategories();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(categories);
        }
    }
}
