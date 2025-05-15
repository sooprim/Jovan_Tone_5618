using Data.Entities;
using Service.DTOs;
using Service.Services;
using Xunit;

namespace Tests.Integration;

public class DiscountIntegrationTests : TestBase
{
    private readonly DiscountService _service;

    public DiscountIntegrationTests()
    {
        _service = new DiscountService(Context, Mapper);
    }

    [Fact]
    public async Task CalculateDiscount_WithValidBasket_ShouldApplyDiscount()
    {
        await ClearDatabase();

        var category = new Category { Name = "Test Category" };
        await Context.Categories.AddAsync(category);
        await Context.SaveChangesAsync();

        var products = new[]
        {
            new Product
            {
                Name = "Product 1",
                Price = 100m,
                Quantity = 10,
                CategoryId = category.Id
            },
            new Product
            {
                Name = "Product 2",
                Price = 200m,
                Quantity = 5,
                CategoryId = category.Id
            }
        };

        await Context.Products.AddRangeAsync(products);
        await Context.SaveChangesAsync();

        var basketItems = new List<BasketItemDto>
        {
            new BasketItemDto { ProductId = products[0].Id, Quantity = 2 },
            new BasketItemDto { ProductId = products[1].Id, Quantity = 1 }
        };

        var result = await _service.CalculateBasketDiscount(basketItems);

        Assert.NotNull(result);
        Assert.Equal(400m, result.TotalBeforeDiscount);
        Assert.True(result.DiscountApplied);
        Assert.Equal(10m, result.DiscountAmount);
        Assert.Equal(390m, result.TotalAfterDiscount);
    }

    [Fact]
    public async Task CalculateDiscount_WithInsufficientStock_ShouldThrowException()
    {
        await ClearDatabase();

        var category = new Category { Name = "Test Category" };
        await Context.Categories.AddAsync(category);
        await Context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Limited Stock Product",
            Price = 100m,
            Quantity = 5,
            CategoryId = category.Id
        };

        await Context.Products.AddAsync(product);
        await Context.SaveChangesAsync();

        var basketItems = new List<BasketItemDto>
        {
            new BasketItemDto { ProductId = product.Id, Quantity = 10 }
        };

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.CalculateBasketDiscount(basketItems));
    }
} 