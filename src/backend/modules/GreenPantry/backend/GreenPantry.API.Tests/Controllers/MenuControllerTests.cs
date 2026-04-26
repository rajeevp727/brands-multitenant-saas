using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using GreenPantry.API.Controllers;
using GreenPantry.Application.DTOs.Menu;
using GreenPantry.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreenPantry.API.Tests.Controllers
{
    public class MenuControllerTests
    {
        private readonly Mock<IMenuService> _mockMenuService;
        private readonly MenuController _controller;

        public MenuControllerTests()
        {
            _mockMenuService = new Mock<IMenuService>();
            _controller = new MenuController(_mockMenuService.Object);
        }

        [Fact]
        public async Task GetMenuByRestaurant_ReturnsOkWithMenu()
        {
            // Arrange
            var restaurantId = "rest_123";
            var menu = new List<MenuCategoryDto> { new MenuCategoryDto { Category = "Category 1" } };
            _mockMenuService.Setup(s => s.GetMenuByRestaurantIdAsync(restaurantId)).ReturnsAsync(menu);

            // Act
            var result = await _controller.GetMenuByRestaurant(restaurantId);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(menu);
        }

        [Fact]
        public async Task GetMenuItem_WithValidId_ReturnsOk()
        {
            // Arrange
            var id = "item_123";
            var item = new MenuItemDto { Id = id, Name = "Item 1" };
            _mockMenuService.Setup(s => s.GetMenuItemByIdAsync(id)).ReturnsAsync(item);

            // Act
            var result = await _controller.GetMenuItem(id);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(item);
        }

        [Fact]
        public async Task GetMenuItem_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var id = "invalid";
            _mockMenuService.Setup(s => s.GetMenuItemByIdAsync(id)).ReturnsAsync((MenuItemDto)null);

            // Act
            var result = await _controller.GetMenuItem(id);

            // Assert
            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task CreateMenuItem_ReturnsCreatedAtAction()
        {
            // Arrange
            var itemDto = new MenuItemDto { Name = "New Item" };
            var createdItem = new MenuItemDto { Id = "item_new", Name = "New Item" };
            _mockMenuService.Setup(s => s.CreateMenuItemAsync(itemDto)).ReturnsAsync(createdItem);

            // Act
            var result = await _controller.CreateMenuItem(itemDto);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Value.Should().BeEquivalentTo(createdItem);
        }
    }
}
