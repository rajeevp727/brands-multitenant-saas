using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SaaS.Api.Controllers;
using SaaS.Application.DTOs;
using SaaS.Application.Interfaces;
using SaaS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace SaaS.Api.Tests.Controllers
{
    public class NotificationsControllerTests
    {
        private readonly Mock<INotificationService> _mockService;
        private readonly NotificationsController _controller;

        public NotificationsControllerTests()
        {
            _mockService = new Mock<INotificationService>();
            _controller = new NotificationsController(_mockService.Object);
            
            // Setup User Claims
            var userId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim("tenantid", tenantId.ToString())
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task GetNotifications_ReturnsOkWithList()
        {
            // Arrange
            var notifications = new List<NotificationDto> { new NotificationDto { Title = "Test" } };
            _mockService.Setup(s => s.GetNotificationsAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid?>(), It.IsAny<bool>()))
                .ReturnsAsync(notifications);

            // Act
            var result = await _controller.GetNotifications();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(notifications);
        }

        [Fact]
        public async Task GetUnreadCount_ReturnsOkWithCount()
        {
            // Arrange
            _mockService.Setup(s => s.GetUnreadCountAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Guid?>()))
                .ReturnsAsync(5);

            // Act
            var result = await _controller.GetUnreadCount();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().Be(5);
        }

        [Fact]
        public async Task MarkAsRead_ReturnsNoContent()
        {
            // Act
            var result = await _controller.MarkAsRead(Guid.NewGuid());

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task CreateNotification_ReturnsCreatedAtAction()
        {
            // Arrange
            var request = new CreateNotificationRequest { Title = "New", Message = "Body" };
            var createdDto = new NotificationDto { Id = Guid.NewGuid(), Title = "New" };
            _mockService.Setup(s => s.CreateNotificationAsync(It.IsAny<Notification>()))
                .ReturnsAsync(createdDto);

            // Act
            var result = await _controller.CreateNotification(request);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>();
        }
    }
}
