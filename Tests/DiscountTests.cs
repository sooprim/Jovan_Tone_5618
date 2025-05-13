using AutoMapper;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Service.DTOs;
using Service.Mapping;
using Service.Services;
using Xunit;

namespace Tests;

public class DiscountTests
{
    private readonly IMapper _mapper;
    private readonly string _dbName;

    public DiscountTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MappingProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _dbName = $"TestDb_Discounts_{Guid.NewGuid()}"; // Unique database for each test run
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
    public async Task CalculateBasketDiscount_WithValidProducts_ShouldApplyDiscount()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await ClearDatabase(context);

        var category = new Category { Name = "Test Category" };
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        var products = new List<Product>
        {
            new Product
            {
                Name = "Product 1",
                Description = "Description 1",
                Price = 100m,
                Quantity = 10,
                CategoryId = category.Id
            },
            new Product
            {
                Name = "Product 2",
                Description = "Description 2",
                Price = 200m,
                Quantity = 5,
                CategoryId = category.Id
            }
        };
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        var service = new DiscountService(context, _mapper);
        var basketItems = new List<BasketItemDto>
        {
            new BasketItemDto { ProductId = products[0].Id + 1, Quantity = 2 }, // External ID is internal ID + 1
            new BasketItemDto { ProductId = products[1].Id + 1, Quantity = 1 }  // External ID is internal ID + 1
        };

        // Act
        var result = await service.CalculateBasketDiscount(basketItems);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(400m, result.TotalBeforeDiscount); // (100 * 2) + (200 * 1)
        Assert.True(result.DiscountApplied);
        Assert.Equal(40m, result.DiscountAmount); // 10% of 400
        Assert.Equal(360m, result.TotalAfterDiscount);
    }

    [Fact]
    public async Task CalculateBasketDiscount_WithInvalidProduct_ShouldThrowException()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        var service = new DiscountService(context, _mapper);
        var basketItems = new List<BasketItemDto>
        {
            new BasketItemDto { ProductId = 999, Quantity = 1 }
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.CalculateBasketDiscount(basketItems));
    }

    [Fact]
    public async Task CalculateBasketDiscount_WithInsufficientQuantity_ShouldThrowException()
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
            Price = 100m,
            Quantity = 5,
            CategoryId = category.Id
        };
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();

        var service = new DiscountService(context, _mapper);
        var basketItems = new List<BasketItemDto>
        {
            new BasketItemDto { ProductId = product.Id + 1, Quantity = 10 } // External ID is internal ID + 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.CalculateBasketDiscount(basketItems));
    }
} 