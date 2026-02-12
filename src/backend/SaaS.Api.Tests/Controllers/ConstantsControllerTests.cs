using Moq;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using SaaS.Api.Controllers;
using SaaS.Domain.Entities;
using SaaS.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SaaS.Api.Tests.Controllers
{
    public class ConstantsControllerTests
    {
        private readonly Mock<IGenericRepository<AppConstant>> _mockRepo;
        private readonly Mock<ITenantProvider> _mockTenantProvider;
        private readonly ConstantsController _controller;

        public ConstantsControllerTests()
        {
            _mockRepo = new Mock<IGenericRepository<AppConstant>>();
            _mockTenantProvider = new Mock<ITenantProvider>();
            _controller = new ConstantsController(_mockRepo.Object, _mockTenantProvider.Object);
        }

        [Fact]
        public async Task GetConstants_ReturnsMergedConstants()
        {
            // Arrange
            _mockTenantProvider.Setup(p => p.GetTenantId()).Returns("tenant_123");
            var constants = new List<AppConstant>
            {
                new AppConstant { ConstantKey = "K1", ConstantValue = "GlobalV1", TenantId = null },
                new AppConstant { ConstantKey = "K2", ConstantValue = "GlobalV2", TenantId = null },
                new AppConstant { ConstantKey = "K2", ConstantValue = "TenantV2", TenantId = "tenant_123" },
                new AppConstant { ConstantKey = "K3", ConstantValue = "TenantV3", TenantId = "tenant_123" }
            };
            _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(constants);

            // Act
            var result = await _controller.GetConstants();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var values = okResult.Value as Dictionary<string, string>;
            
            values["K1"].Should().Be("GlobalV1");
            values["K2"].Should().Be("TenantV2");
            values["K3"].Should().Be("TenantV3");
        }
    }
}
