using FluentAssertions;
using GreenPantry.API.Controllers;
using GreenPantry.API.Tests.Helpers;
using GreenPantry.Application.DTOs.Order;
using GreenPantry.Application.Interfaces;
using GreenPantry.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace GreenPantry.API.Tests.Controllers;

public class OrdersControllerTests
{
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly OrdersController _controller;
    private readonly TestDataBuilder _dataBuilder;
    private const string CurrentUserId = "user_123";

    public OrdersControllerTests()
    {
        _mockOrderService = new Mock<IOrderService>();
        _controller = new OrdersController(_mockOrderService.Object);
        _dataBuilder = new TestDataBuilder();

        // Setup HttpContext with Claims
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, CurrentUserId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task CreateOrder_WithValidData_ReturnsCreated()
    {
        // Arrange
        var request = _dataBuilder.Create<CreateOrderRequest>();
        var orderDto = _dataBuilder.Create<OrderDto>();
        _mockOrderService.Setup(s => s.CreateOrderAsync(request, CurrentUserId)).ReturnsAsync(orderDto);

        // Act
        var result = await _controller.CreateOrder(request);

        // Assert
        result.Result.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task GetOrder_WithValidId_ReturnsOk()
    {
        // Arrange
        var id = "ord_123";
        var order = _dataBuilder.Create<OrderDto>();
        order.UserId = CurrentUserId;
        _mockOrderService.Setup(s => s.GetOrderByIdAsync(id)).ReturnsAsync(order);

        // Act
        var result = await _controller.GetOrder(id);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task UpdateOrderStatus_ReturnsOk()
    {
        // Arrange
        var id = "ord_123";
        var request = new UpdateOrderStatusRequest { Status = OrderStatus.Delivered };
        var order = _dataBuilder.Create<OrderDto>();
        _mockOrderService.Setup(s => s.UpdateOrderStatusAsync(id, request)).ReturnsAsync(order);

        // Act
        var result = await _controller.UpdateOrderStatus(id, request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }
}
