using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Vendor.API.Controllers;
using Vendor.Application.DTOs.Order;
using Vendor.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Vendor.API.Tests.Controllers
{
    public class OrderControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<ILogger<OrderController>> _mockLogger;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            _mockOrderService = new Mock<IOrderService>();
            _mockLogger = new Mock<ILogger<OrderController>>();
            _controller = new OrderController(_mockOrderService.Object, _mockLogger.Object);
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("VendorId", "123")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task GetOrders_ReturnsOkWithList()
        {
            // Arrange
            var orders = new List<OrderDto> { new OrderDto { OrderId = 1 } };
            _mockOrderService.Setup(s => s.GetVendorOrdersAsync(123)).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetOrders();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(orders);
        }

        [Fact]
        public async Task AcceptOrder_Valid_ReturnsOk()
        {
            // Arrange
            _mockOrderService.Setup(s => s.AcceptOrderAsync(1, 123)).ReturnsAsync(true);

            // Act
            var result = await _controller.AcceptOrder(1);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AcceptOrder_Invalid_ReturnsBadRequest()
        {
            // Arrange
            _mockOrderService.Setup(s => s.AcceptOrderAsync(1, 123)).ReturnsAsync(false);

            // Act
            var result = await _controller.AcceptOrder(1);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
