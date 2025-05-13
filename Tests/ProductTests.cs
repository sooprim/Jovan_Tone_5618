using AutoMapper;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Service.DTOs;
using Service.Mapping;
using Service.Services;
using Xunit;

namespace Tests;

public class ProductTests
{
    private readonly IMapper _mapper;
    private readonly string _dbName;

    public ProductTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MappingProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _dbName = $"TestDb_Products_{Guid.NewGuid()}"; // Unique database for each test run
    }

    private DbContextOptions<ApplicationDbContext> GetDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: _dbName)
            .Options;
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnAllProducts()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        // Clear existing data
        context.Products.RemoveRange(context.Products);
        context.Categories.RemoveRange(context.Categories);
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

        var service = new ProductService(context, _mapper);

        // Act
        var result = await service.GetAllProductsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Test Product", result[0].Name);
        Assert.Equal("Test Category", result[0].CategoryName);
        Assert.Equal(1, result[0].Id); // External ID should be 1
    }

    [Fact]
    public async Task GetProductByIdAsync_WithValidId_ShouldReturnProduct()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        // Clear existing data
        context.Products.RemoveRange(context.Products);
        context.Categories.RemoveRange(context.Categories);
        await context.SaveChangesAsync();

        var category = new Category { Id = 0, Name = "Test Category" };
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

        var service = new ProductService(context, _mapper);

        // Act
        var result = await service.GetProductByIdAsync(1); // External ID 1 maps to internal ID 0

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal(9.99m, result.Price);
        Assert.Equal(1, result.Id); // External ID should be 1
    }

    [Fact]
    public async Task GetProductByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        using var context = new ApplicationDbContext(GetDbContextOptions());
        var service = new ProductService(context, _mapper);

        var result = await service.GetProductByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProductAsync_WithValidData_ShouldCreateProduct()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        // Clear existing data
        context.Products.RemoveRange(context.Products);
        context.Categories.RemoveRange(context.Categories);
        await context.SaveChangesAsync();

        var category = new Category { Id = 0, Name = "Test Category" };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper);
        var productDto = new ProductCreateUpdateDto
        {
            Name = "New Product",
            Description = "New Description",
            Price = 19.99m,
            Quantity = 5,
            CategoryId = 1 // External ID 1 maps to internal ID 0
        };

        // Act
        var result = await service.CreateProductAsync(productDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product", result.Name);
        Assert.Equal(19.99m, result.Price);
        Assert.Equal(5, result.Quantity);
        Assert.Equal(1, result.Id); // External ID should be 1
    }

    [Fact]
    public async Task UpdateProductAsync_WithValidData_ShouldUpdateProduct()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        // Clear existing data
        context.Products.RemoveRange(context.Products);
        context.Categories.RemoveRange(context.Categories);
        await context.SaveChangesAsync();

        var category = new Category { Id = 0, Name = "Test Category" };
        var product = new Product
        {
            Id = 0,
            Name = "Original Name",
            Description = "Original Description",
            Price = 9.99m,
            Quantity = 10,
            Category = category
        };
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper);
        var updateDto = new ProductCreateUpdateDto
        {
            Name = "Updated Name",
            Description = "Updated Description",
            Price = 29.99m,
            Quantity = 15,
            CategoryId = 1 // External ID 1 maps to internal ID 0
        };

        // Act
        var result = await service.UpdateProductAsync(1, updateDto); // External ID 1 maps to internal ID 0

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal(29.99m, result.Price);
        Assert.Equal(15, result.Quantity);
        Assert.Equal(1, result.Id); // External ID should be 1
    }

    [Fact]
    public async Task DeleteProductAsync_WithValidId_ShouldDeleteProduct()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        // Clear existing data
        context.Products.RemoveRange(context.Products);
        context.Categories.RemoveRange(context.Categories);
        await context.SaveChangesAsync();

        var category = new Category { Id = 0, Name = "Test Category" };
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

        var service = new ProductService(context, _mapper);

        // Act
        var result = await service.DeleteProductAsync(1); // External ID 1 maps to internal ID 0

        // Assert
        Assert.True(result);
        Assert.Empty(await context.Products.ToListAsync());
    }

    [Fact]
    public async Task DeleteProductAsync_WithInvalidId_ShouldReturnFalse()
    {
        using var context = new ApplicationDbContext(GetDbContextOptions());
        var service = new ProductService(context, _mapper);

        var result = await service.DeleteProductAsync(999);

        Assert.False(result);
    }
} 