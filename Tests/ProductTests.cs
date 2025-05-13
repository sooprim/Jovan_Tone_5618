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

    private async Task ClearDatabase(ApplicationDbContext context)
    {
        context.Products.RemoveRange(context.Products);
        context.Categories.RemoveRange(context.Categories);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnAllProducts()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await ClearDatabase(context);

        var category = new Category { Name = "Test Category", Description = "Test Description" };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 9.99m,
            Quantity = 10,
            CategoryId = category.Id
        };
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper);

        // Act
        var result = await service.GetAllProductsAsync();

        // Assert
        Assert.Single(result);
        Assert.Equal("Test Product", result[0].Name);
        Assert.Equal("Test Category", result[0].CategoryName);
        Assert.Equal(product.Id + 1, result[0].Id); // External ID should be internal ID + 1
        Assert.Equal(category.Id + 1, result[0].CategoryId); // External CategoryId should be internal ID + 1
    }

    [Fact]
    public async Task GetProductByIdAsync_WithValidId_ShouldReturnProduct()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await ClearDatabase(context);

        var category = new Category { Name = "Test Category" };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 9.99m,
            Quantity = 10,
            CategoryId = category.Id
        };
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper);

        // Act
        var result = await service.GetProductByIdAsync(product.Id + 1); // External ID is internal ID + 1

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal(9.99m, result.Price);
        Assert.Equal(product.Id + 1, result.Id);
        Assert.Equal(category.Id + 1, result.CategoryId);
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
        await ClearDatabase(context);

        var category = new Category { Name = "Test Category" };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper);
        var productDto = new ProductCreateUpdateDto
        {
            Name = "New Product",
            Description = "New Description",
            Price = 19.99m,
            Quantity = 5,
            CategoryId = category.Id + 1 // External ID is internal ID + 1
        };

        // Act
        var result = await service.CreateProductAsync(productDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product", result.Name);
        Assert.Equal(19.99m, result.Price);
        Assert.Equal(5, result.Quantity);
        Assert.Equal(category.Id + 1, result.CategoryId);

        // Verify in database
        var dbProduct = await context.Products.Include(p => p.Category).FirstAsync();
        Assert.Equal("New Product", dbProduct.Name);
        Assert.Equal(category.Id, dbProduct.CategoryId);
    }

    [Fact]
    public async Task UpdateProductAsync_WithValidData_ShouldUpdateProduct()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await ClearDatabase(context);

        var category = new Category { Name = "Test Category" };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Original Name",
            Description = "Original Description",
            Price = 9.99m,
            Quantity = 10,
            CategoryId = category.Id
        };
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper);
        var updateDto = new ProductCreateUpdateDto
        {
            Name = "Updated Name",
            Description = "Updated Description",
            Price = 29.99m,
            Quantity = 15,
            CategoryId = category.Id + 1 // External ID is internal ID + 1
        };

        // Act
        var result = await service.UpdateProductAsync(product.Id + 1, updateDto); // External ID is internal ID + 1

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
        Assert.Equal(29.99m, result.Price);
        Assert.Equal(15, result.Quantity);
        Assert.Equal(product.Id + 1, result.Id);
        Assert.Equal(category.Id + 1, result.CategoryId);

        // Verify in database
        var dbProduct = await context.Products.Include(p => p.Category).FirstAsync();
        Assert.Equal("Updated Name", dbProduct.Name);
        Assert.Equal(category.Id, dbProduct.CategoryId);
    }

    [Fact]
    public async Task DeleteProductAsync_WithValidId_ShouldDeleteProduct()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await ClearDatabase(context);

        var category = new Category { Name = "Test Category" };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 9.99m,
            Quantity = 10,
            CategoryId = category.Id
        };
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var service = new ProductService(context, _mapper);

        // Act
        var result = await service.DeleteProductAsync(product.Id + 1); // External ID is internal ID + 1

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