using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SaaS.Api.Controllers;
using SaaS.Api.Tests.Helpers;
using SaaS.Application.Common;
using SaaS.Application.DTOs;
using SaaS.Domain.Entities;
using SaaS.Infrastructure.Persistence;
using SaaS.Domain.Interfaces;
using Xunit;

namespace SaaS.Api.Tests.Controllers;

public class BrandsControllerTests
{
    private readonly Mock<IGenericService<BrandDto, Brand>> _mockBrandService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ApplicationDbContext _context;
    private readonly BrandsController _controller;
    private readonly TestDataBuilder _dataBuilder;

    public BrandsControllerTests()
    {
        _mockBrandService = new Mock<IGenericService<BrandDto, Brand>>();
        _mockMapper = new Mock<IMapper>();
        
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
            
        var mockTenantProvider = new Mock<ITenantProvider>();
        mockTenantProvider.Setup(p => p.GetTenantId()).Returns("test-tenant");
        _context = new ApplicationDbContext(options, mockTenantProvider.Object);
        
        _controller = new BrandsController(_mockBrandService.Object, _context, _mockMapper.Object);
        _dataBuilder = new TestDataBuilder();
    }

    [Fact]
    public async Task GetAllBrands_ReturnsOk_WithMappedBrandsAndPorts()
    {
        // Arrange
        var brands = new List<Brand>
        {
            new Brand { Id = Guid.NewGuid(), Name = "Brand 1", ConfigJson = "{\"port\": \"5173\"}", TenantId = "tenant-1" },
            new Brand { Id = Guid.NewGuid(), Name = "Brand 2", ConfigJson = "{\"port\": \"5174\"}", TenantId = "tenant-2" }
        };
        _context.Brands.AddRange(brands);
        await _context.SaveChangesAsync();

        var brandDtos = brands.Select(b => new BrandDto { Id = b.Id, Name = b.Name, ConfigJson = b.ConfigJson }).ToList();
        _mockMapper.Setup(m => m.Map<List<BrandDto>>(It.IsAny<List<Brand>>())).Returns(brandDtos);

        // Act
        var result = await _controller.GetAllBrands();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        var returnedDtos = okResult.Value as List<BrandDto>;
        returnedDtos.Should().HaveCount(2);
        returnedDtos[0].Port.Should().Be("5173");
        returnedDtos[1].Port.Should().Be("5174");
    }

    [Fact]
    public async Task GetCurrentBrand_ReturnsOk_WhenBrandExists()
    {
        // Arrange
        var brandDto = _dataBuilder.Create<BrandDto>();
        _mockBrandService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<BrandDto> { brandDto });

        // Act
        var result = await _controller.GetCurrentBrand();

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(brandDto);
    }

    [Fact]
    public async Task GetCurrentBrand_ReturnsNotFound_WhenBrandDoesNotExist()
    {
        // Arrange
        _mockBrandService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<BrandDto>());

        // Act
        var result = await _controller.GetCurrentBrand();

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }
}
