using AutoMapper;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Service.DTOs;
using Service.Services;
using Xunit;

namespace Tests.Unit;

public class CategoryUnitTests
{
    private readonly Mock<DbSet<Category>> _mockCategoryDbSet;
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoryService _service;

    public CategoryUnitTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _mockCategoryDbSet = new Mock<DbSet<Category>>();
        _mockContext = new Mock<ApplicationDbContext>(options);
        _mockMapper = new Mock<IMapper>();
        _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
        _service = new CategoryService(_mockContext.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ShouldMapAndReturnCategory()
    {
        var categoryId = 1;
        var category = new Category { Id = categoryId, Name = "Test Category", Description = "Test Description" };
        var categoryDto = new CategoryDto { Id = categoryId, Name = "Test Category", Description = "Test Description" };

        var categories = new List<Category> { category }.AsQueryable();
        var mockDbSet = categories.BuildMockDbSet();
        _mockContext.Setup(c => c.Categories).Returns(mockDbSet.Object);
        _mockMapper.Setup(m => m.Map<CategoryDto>(It.IsAny<Category>())).Returns(categoryDto);

        var result = await _service.GetCategoryByIdAsync(categoryId);

        Assert.NotNull(result);
        Assert.Equal(categoryId, result.Id);
        Assert.Equal(category.Name, result.Name);
        _mockMapper.Verify(m => m.Map<CategoryDto>(It.IsAny<Category>()), Times.Once);
    }
} 