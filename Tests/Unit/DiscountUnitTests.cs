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

public class DiscountUnitTests
{
    private readonly Mock<DbSet<Product>> _mockProductDbSet;
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly DiscountService _service;

    public DiscountUnitTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _mockProductDbSet = new Mock<DbSet<Product>>();
        _mockContext = new Mock<ApplicationDbContext>(options);
        _mockMapper = new Mock<IMapper>();
        _mockContext.Setup(c => c.Products).Returns(_mockProductDbSet.Object);
        _service = new DiscountService(_mockContext.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CalculateBasketDiscount_ShouldApplyCorrectDiscount()
    {
        var category = new Category { Id = 1, Name = "Test Category" };
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Product 1", Price = 100m, Quantity = 10, CategoryId = 1, Category = category },
            new Product { Id = 2, Name = "Product 2", Price = 200m, Quantity = 5, CategoryId = 1, Category = category }
        };

        var basketItems = new List<BasketItemDto>
        {
            new BasketItemDto { ProductId = 1, Quantity = 2 },
            new BasketItemDto { ProductId = 2, Quantity = 1 }
        };

        var mockDbSet = products.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(c => c.Products).Returns(mockDbSet.Object);

        var result = await _service.CalculateBasketDiscount(basketItems);

        Assert.NotNull(result);
        Assert.Equal(400m, result.TotalBeforeDiscount);
        Assert.True(result.DiscountApplied);
        Assert.Equal(10m, result.DiscountAmount);
        Assert.Equal(390m, result.TotalAfterDiscount);
    }
} 