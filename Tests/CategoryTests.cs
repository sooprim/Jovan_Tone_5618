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
        _dbName = $"TestDb_Categories_{Guid.NewGuid()}";
    }

    private DbContextOptions<ApplicationDbContext> GetDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: _dbName)
            .Options;
    }

    private async Task ClearDatabase(ApplicationDbContext context)
    {
        context.Products.RemoveRange(context.Products);
        context.Categories.RemoveRange(context.Categories);
        await context.SaveChangesAsync();
        
        // Reset the database state
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnAllCategories()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await ClearDatabase(context);

        var category = new Category
        {
            Name = "Test Category",
            Description = "Test Description"
        };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Test Product",
            CategoryId = category.Id,
            Quantity = 5
        };
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var service = new CategoryService(context, _mapper);

        // Act
        var result = await service.GetAllCategoriesAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Test Category", result[0].Name);
        Assert.Equal(category.Id, result[0].Id);
        Assert.Equal(5, result[0].Quantity);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WithValidId_ShouldReturnCategory()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await ClearDatabase(context);

        var category = new Category
        {
            Name = "Test Category",
            Description = "Test Description"
        };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Test Product",
            CategoryId = category.Id,
            Quantity = 5
        };
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var service = new CategoryService(context, _mapper);

        // Act
        var result = await service.GetCategoryByIdAsync(category.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Category", result.Name);
        Assert.Equal(category.Id, result.Id);
        Assert.Equal(5, result.Quantity);
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
        await ClearDatabase(context);

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
        Assert.True(result.Id > 0);

        // Verify in database
        var dbCategory = await context.Categories.FirstAsync();
        Assert.Equal("New Category", dbCategory.Name);
        Assert.Equal(dbCategory.Id, result.Id);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WithValidData_ShouldUpdateCategory()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await ClearDatabase(context);

        var category = new Category
        {
            Name = "Original Name",
            Description = "Original Description"
        };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context, _mapper);
        var updateDto = new UpdateCategoryDto
        {
            Name = "Updated Name",
            Description = "Updated Description"
        };

        // Act
        var result = await service.UpdateCategoryAsync(category.Id, updateDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal("Updated Description", result.Description);
        Assert.Equal(category.Id, result.Id);

        // Verify in database
        var dbCategory = await context.Categories.FirstAsync();
        Assert.Equal("Updated Name", dbCategory.Name);
        Assert.Equal(dbCategory.Id, result.Id);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithValidId_ShouldDeleteCategory()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await ClearDatabase(context);

        var category = new Category
        {
            Name = "Test Category",
            Description = "Test Description"
        };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var service = new CategoryService(context, _mapper);

        // Act
        var result = await service.DeleteCategoryAsync(category.Id);

        // Assert
        Assert.True(result);
        Assert.Empty(await context.Categories.ToListAsync());
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithProducts_ShouldThrowException()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await ClearDatabase(context);

        var category = new Category
        {
            Name = "Test Category",
            Description = "Test Description"
        };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Test Product",
            CategoryId = category.Id
        };
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var service = new CategoryService(context, _mapper);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.DeleteCategoryAsync(category.Id));
    }
} 