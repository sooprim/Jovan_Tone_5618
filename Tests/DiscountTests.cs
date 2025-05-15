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
        _dbName = $"TestDb_Discounts_{Guid.NewGuid()}";
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
        
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }

    [Fact]
    public async Task CalculateBasketDiscount_WithValidProducts_ShouldApplyDiscount()
    {
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
            new BasketItemDto { ProductId = products[0].Id, Quantity = 2 },
            new BasketItemDto { ProductId = products[1].Id, Quantity = 1 }
        };

        var result = await service.CalculateBasketDiscount(basketItems);

        Assert.NotNull(result);
        Assert.Equal(400m, result.TotalBeforeDiscount);
        Assert.True(result.DiscountApplied);
        Assert.Equal(10m, result.DiscountAmount);
        Assert.Equal(390m, result.TotalAfterDiscount);
    }

    [Fact]
    public async Task CalculateBasketDiscount_WithInvalidProduct_ShouldThrowException()
    {
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await ClearDatabase(context);

        var service = new DiscountService(context, _mapper);
        var basketItems = new List<BasketItemDto>
        {
            new BasketItemDto { ProductId = 999, Quantity = 1 }
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.CalculateBasketDiscount(basketItems));
    }

    [Fact]
    public async Task CalculateBasketDiscount_WithInsufficientQuantity_ShouldThrowException()
    {
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
            new BasketItemDto { ProductId = product.Id, Quantity = 10 }
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.CalculateBasketDiscount(basketItems));
    }
} 