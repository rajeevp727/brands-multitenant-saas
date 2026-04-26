using FluentAssertions;
using GreenPantry.API.Controllers;
using GreenPantry.API.Tests.Helpers;
using GreenPantry.Application.DTOs.Menu;
using GreenPantry.Application.DTOs.Restaurant;
using GreenPantry.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace GreenPantry.API.Tests.Controllers;

public class RestaurantsControllerTests
{
    private readonly Mock<IRestaurantService> _mockRestaurantService;
    private readonly Mock<IMenuService> _mockMenuService;
    private readonly RestaurantsController _controller;
    private readonly TestDataBuilder _dataBuilder;

    public RestaurantsControllerTests()
    {
        _mockRestaurantService = new Mock<IRestaurantService>();
        _mockMenuService = new Mock<IMenuService>();
        _controller = new RestaurantsController(_mockRestaurantService.Object, _mockMenuService.Object);
        _dataBuilder = new TestDataBuilder();
    }

    [Fact]
    public async Task GetRestaurants_ReturnsOk_WithListOfRestaurants()
    {
        // Arrange
        var filter = new RestaurantFilterDto();
        var restaurants = _dataBuilder.CreateMany<RestaurantDto>(2);
        _mockRestaurantService.Setup(s => s.GetRestaurantsAsync(filter)).ReturnsAsync(restaurants);

        // Act
        var result = await _controller.GetRestaurants(filter);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(restaurants);
    }

    [Fact]
    public async Task GetRestaurant_WithValidId_ReturnsOk()
    {
        // Arrange
        var id = "rest_123";
        var restaurant = _dataBuilder.Create<RestaurantDetailDto>();
        _mockRestaurantService.Setup(s => s.GetRestaurantByIdAsync(id)).ReturnsAsync(restaurant);

        // Act
        var result = await _controller.GetRestaurant(id);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetRestaurant_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var id = "invalid";
        _mockRestaurantService.Setup(s => s.GetRestaurantByIdAsync(id)).ReturnsAsync((RestaurantDetailDto)null);

        // Act
        var result = await _controller.GetRestaurant(id);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetRestaurantMenu_ReturnsOk_WithMenu()
    {
        // Arrange
        var id = "rest_123";
        var menu = _dataBuilder.CreateMany<MenuCategoryDto>(3);
        _mockMenuService.Setup(s => s.GetMenuByRestaurantIdAsync(id)).ReturnsAsync(menu);

        // Act
        var result = await _controller.GetRestaurantMenu(id);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult.Value.Should().BeEquivalentTo(menu);
    }
}
