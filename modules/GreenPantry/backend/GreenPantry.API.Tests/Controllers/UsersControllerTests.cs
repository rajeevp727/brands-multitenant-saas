using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using GreenPantry.API.Controllers;
using GreenPantry.Application.DTOs.Auth;
using GreenPantry.Application.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace GreenPantry.API.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UsersController(_mockUserService.Object);
            
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("userId", "user_123")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task GetCurrentUser_ReturnsOk()
        {
            // Arrange
            var userDto = new UserDto { Id = "user_123", Email = "test@example.com" };
            _mockUserService.Setup(s => s.GetUserByIdAsync("user_123")).ReturnsAsync(userDto);

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(userDto);
        }

        [Fact]
        public async Task UpdateMyAddress_ReturnsNoContent()
        {
            // Arrange
            _mockUserService.Setup(s => s.UpdateUserAddressAsync("user_123", It.IsAny<AddressDto>())).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateMyAddress(new AddressDto());

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }
    }
}
