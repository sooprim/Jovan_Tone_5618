using AutoMapper;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Service.DTOs;
using Service.Mapping;
using Service.Services;
using Xunit;

namespace Tests;

public class CategoryTests
{
    private readonly IMapper _mapper;
    private readonly string _dbName;

    public CategoryTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MappingProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _dbName = $"TestDb_Categories_{Guid.NewGuid()}"; // Unique database for each test run
    }

    private DbContextOptions<ApplicationDbContext> GetDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: _dbName)
            .Options;
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnAllCategories()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        // Clear existing data
        context.Categories.RemoveRange(context.Categories);
        await context.SaveChangesAsync();

        var category = new Category { Id = 0, Name = "Test Category", Description = "Test Description" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context, _mapper);

        // Act
        var result = await service.GetAllCategoriesAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Test Category", result[0].Name);
        Assert.Equal("Test Description", result[0].Description);
        Assert.Equal(1, result[0].Id); // External ID should be 1
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WithValidId_ShouldReturnCategory()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        // Clear existing data
        context.Categories.RemoveRange(context.Categories);
        await context.SaveChangesAsync();

        var category = new Category { Id = 0, Name = "Test Category", Description = "Test Description" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context, _mapper);

        // Act
        var result = await service.GetCategoryByIdAsync(1); // External ID 1 maps to internal ID 0

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Category", result.Name);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(1, result.Id); // External ID should be 1
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        var service = new CategoryService(context, _mapper);

        // Act
        var result = await service.GetCategoryByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithValidData_ShouldCreateCategory()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        // Clear existing data
        context.Categories.RemoveRange(context.Categories);
        await context.SaveChangesAsync();

        var service = new CategoryService(context, _mapper);
        var categoryDto = new CreateCategoryDto
        {
            Name = "New Category",
            Description = "New Description"
        };

        // Act
        var result = await service.CreateCategoryAsync(categoryDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Category", result.Name);
        Assert.Equal("New Description", result.Description);
        Assert.Equal(1, result.Id); // First category, external ID should be 1
    }

    [Fact]
    public async Task UpdateCategoryAsync_WithValidData_ShouldUpdateCategory()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        // Clear existing data
        context.Categories.RemoveRange(context.Categories);
        await context.SaveChangesAsync();

        var category = new Category { Id = 0, Name = "Original Name", Description = "Original Description" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context, _mapper);
        var updateDto = new UpdateCategoryDto
        {
            Name = "Updated Name",
            Description = "Updated Description"
        };

        // Act
        var result = await service.UpdateCategoryAsync(1, updateDto); // External ID 1 maps to internal ID 0

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal("Updated Description", result.Description);
        Assert.Equal(1, result.Id); // External ID should be 1
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithValidId_AndNoProducts_ShouldDeleteCategory()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        // Clear existing data
        context.Categories.RemoveRange(context.Categories);
        await context.SaveChangesAsync();

        var category = new Category { Id = 0, Name = "Test Category", Description = "Test Description" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context, _mapper);

        // Act
        var result = await service.DeleteCategoryAsync(1); // External ID 1 maps to internal ID 0

        // Assert
        Assert.True(result);
        Assert.Empty(await context.Categories.ToListAsync());
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithProducts_ShouldThrowException()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        // Clear existing data
        context.Categories.RemoveRange(context.Categories);
        context.Products.RemoveRange(context.Products);
        await context.SaveChangesAsync();

        var category = new Category { Id = 0, Name = "Test Category", Description = "Test Description" };
        var product = new Product
        {
            Id = 0,
            Name = "Test Product",
            Description = "Test Description",
            Price = 9.99m,
            Quantity = 10,
            Category = category
        };
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new CategoryService(context, _mapper);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.DeleteCategoryAsync(1) // External ID 1 maps to internal ID 0
        );
    }
} 