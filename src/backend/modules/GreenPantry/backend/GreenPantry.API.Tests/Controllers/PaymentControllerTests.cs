using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using GreenPantry.API.Controllers;
using GreenPantry.Application.DTOs.Payment;
using GreenPantry.Application.Interfaces;
using GreenPantry.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GreenPantry.API.Tests.Controllers
{
    public class PaymentControllerTests
    {
        private readonly Mock<IPaymentFactoryService> _mockFactory;
        private readonly Mock<IPaymentService> _mockPaymentService;
        private readonly PaymentController _controller;

        public PaymentControllerTests()
        {
            _mockFactory = new Mock<IPaymentFactoryService>();
            _mockPaymentService = new Mock<IPaymentService>();
            
            _mockFactory.Setup(f => f.GetPaymentService(It.IsAny<PaymentProvider>()))
                        .Returns(_mockPaymentService.Object);
            _mockFactory.Setup(f => f.IsProviderEnabledAsync(It.IsAny<PaymentProvider>()))
                        .ReturnsAsync(true);

            _controller = new PaymentController(_mockFactory.Object);
        }

        [Fact]
        public async Task CreatePayment_ValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new PaymentRequestDto { Provider = PaymentProvider.Razorpay };
            var response = new PaymentResponseDto { PaymentId = "pay_123" };
            _mockPaymentService.Setup(s => s.CreatePaymentAsync(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.CreatePayment(request);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task GetEnabledProviders_ReturnsList()
        {
            // Arrange
            var providers = new List<PaymentProvider> { PaymentProvider.Razorpay };
            _mockFactory.Setup(f => f.GetEnabledProvidersAsync()).ReturnsAsync(providers);

            // Act
            var result = await _controller.GetEnabledProviders();

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(providers);
        }

        [Fact]
        public async Task CreatePayment_ProviderDisabled_ReturnsBadRequest()
        {
            // Arrange
            var request = new PaymentRequestDto { Provider = PaymentProvider.Razorpay };
            _mockFactory.Setup(f => f.IsProviderEnabledAsync(PaymentProvider.Razorpay)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreatePayment(request);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
