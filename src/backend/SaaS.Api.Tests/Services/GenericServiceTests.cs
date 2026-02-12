using Moq;
using AutoMapper;
using SaaS.Application.Common;
using SaaS.Domain.Interfaces;
using FluentAssertions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SaaS.Api.Tests.Services;

public class GenericServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IGenericRepository<TestEntity>> _mockRepo;
    private readonly GenericService<TestDto, TestEntity> _service;

    public GenericServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockRepo = new Mock<IGenericRepository<TestEntity>>();
        
        _mockUnitOfWork.Setup(u => u.Repository<TestEntity>()).Returns(_mockRepo.Object);
        _service = new GenericService<TestDto, TestEntity>(_mockUnitOfWork.Object, _mockMapper.Object);
    }

    public class TestEntity { public Guid Id { get; set; } }
    public class TestDto { public Guid Id { get; set; } }

    [Fact]
    public async Task GetAllAsync_ReturnsMappedDtos()
    {
        // Arrange
        var entities = new List<TestEntity> { new TestEntity() };
        var dtos = new List<TestDto> { new TestDto() };

        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        _mockMapper.Setup(m => m.Map<IEnumerable<TestDto>>(entities)).Returns(dtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.Should().BeEquivalentTo(dtos);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new TestEntity { Id = id };
        var dto = new TestDto { Id = id };

        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
        _mockMapper.Setup(m => m.Map<TestDto>(entity)).Returns(dto);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ReturnsCreatedDto()
    {
        // Arrange
        var dto = new TestDto();
        var entity = new TestEntity();

        _mockMapper.Setup(m => m.Map<TestEntity>(dto)).Returns(entity);
        _mockMapper.Setup(m => m.Map<TestDto>(entity)).Returns(dto);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        result.Should().Be(dto);
        _mockRepo.Verify(r => r.AddAsync(entity), Times.Exactly(1));
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task UpdateAsync_ExistingEntity_CallsUpdateAndSave()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new TestDto { Id = id };
        var entity = new TestEntity { Id = id };

        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

        // Act
        await _service.UpdateAsync(id, dto);

        // Assert
        _mockRepo.Verify(r => r.Update(entity), Times.Exactly(1));
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
    }

    [Fact]
    public async Task DeleteAsync_ExistingEntity_CallsDeleteAndSave()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new TestEntity { Id = id };

        _mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

        // Act
        await _service.DeleteAsync(id);

        // Assert
        _mockRepo.Verify(r => r.Delete(entity), Times.Exactly(1));
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
    }
}
