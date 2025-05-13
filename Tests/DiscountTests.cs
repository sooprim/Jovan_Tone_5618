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
        _dbName = $"TestDb_Discount_{Guid.NewGuid()}"; // Unique database for each test run
    }

    private DbContextOptions<ApplicationDbContext> GetDbContextOptions()
    {
        return new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: _dbName)
            .Options;
    }

    [Fact]
    public async Task CalculateDiscountAsync_SingleProduct_NoDiscount()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await SeedTestData(context);
        var service = new DiscountService(context);

        var basketItems = new List<BasketItemDto>
        {
            new BasketItemDto
            {
                ProductId = 1, // External ID
                ProductName = "Intel Core i9",
                Price = 500m,
                Quantity = 1
            }
        };

        // Act
        var result = await service.CalculateDiscountAsync(basketItems);

        // Assert
        Assert.Equal(500m, result.OriginalTotal);
        Assert.Equal(500m, result.DiscountedTotal);
        Assert.Equal(0m, result.DiscountAmount);
        Assert.Equal("No discount applied", result.DiscountDescription);
    }

    [Fact]
    public async Task CalculateDiscountAsync_MultipleProductsSameCategory_ApplyDiscount()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await SeedTestData(context);
        var service = new DiscountService(context);

        var basketItems = new List<BasketItemDto>
        {
            new BasketItemDto
            {
                ProductId = 1, // External ID
                ProductName = "Intel Core i9",
                Price = 500m,
                Quantity = 1
            },
            new BasketItemDto
            {
                ProductId = 2, // External ID
                ProductName = "AMD Ryzen 9",
                Price = 400m,
                Quantity = 1
            }
        };

        // Act
        var result = await service.CalculateDiscountAsync(basketItems);

        // Assert
        Assert.Equal(900m, result.OriginalTotal);
        Assert.Equal(875m, result.DiscountedTotal);
        Assert.Equal(25m, result.DiscountAmount);
        Assert.Contains("5% off first CPU", result.DiscountDescription);
    }

    [Fact]
    public async Task CalculateDiscountAsync_InsufficientStock_ShouldThrowException()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await SeedTestData(context);
        var service = new DiscountService(context);

        var basketItems = new List<BasketItemDto>
        {
            new BasketItemDto
            {
                ProductId = 1, // External ID
                ProductName = "Intel Core i9",
                Price = 500m,
                Quantity = 10
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await service.CalculateDiscountAsync(basketItems)
        );
    }

    [Fact]
    public async Task CalculateDiscountAsync_MultipleCategories_ApplyDiscountPerCategory()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await SeedTestData(context);
        var service = new DiscountService(context);

        var basketItems = new List<BasketItemDto>
        {
            new BasketItemDto
            {
                ProductId = 1, // External ID
                ProductName = "Intel Core i9",
                Price = 500m,
                Quantity = 1
            },
            new BasketItemDto
            {
                ProductId = 2, // External ID
                ProductName = "AMD Ryzen 9",
                Price = 400m,
                Quantity = 1
            },
            new BasketItemDto
            {
                ProductId = 3, // External ID
                ProductName = "NVIDIA RTX 3080",
                Price = 700m,
                Quantity = 1
            },
            new BasketItemDto
            {
                ProductId = 4, // External ID
                ProductName = "AMD RX 6800",
                Price = 600m,
                Quantity = 1
            }
        };

        // Act
        var result = await service.CalculateDiscountAsync(basketItems);

        // Assert
        Assert.Equal(2200m, result.OriginalTotal);
        Assert.Equal(2140m, result.DiscountedTotal);
        Assert.Equal(60m, result.DiscountAmount);
        Assert.Contains("5% off first CPU", result.DiscountDescription);
        Assert.Contains("5% off first GPU", result.DiscountDescription);
    }

    private async Task SeedTestData(ApplicationDbContext context)
    {
        // Clear existing data
        context.Products.RemoveRange(context.Products);
        context.Categories.RemoveRange(context.Categories);
        await context.SaveChangesAsync();

        var cpuCategory = new Category { Id = 0, Name = "CPU" };
        var gpuCategory = new Category { Id = 1, Name = "GPU" };

        var products = new List<Product>
        {
            new Product
            {
                Id = 0, // Internal ID 0 = External ID 1
                Name = "Intel Core i9",
                Description = "CPU",
                Price = 500m,
                Quantity = 5,
                Category = cpuCategory
            },
            new Product
            {
                Id = 1, // Internal ID 1 = External ID 2
                Name = "AMD Ryzen 9",
                Description = "CPU",
                Price = 400m,
                Quantity = 5,
                Category = cpuCategory
            },
            new Product
            {
                Id = 2, // Internal ID 2 = External ID 3
                Name = "NVIDIA RTX 3080",
                Description = "GPU",
                Price = 700m,
                Quantity = 5,
                Category = gpuCategory
            },
            new Product
            {
                Id = 3, // Internal ID 3 = External ID 4
                Name = "AMD RX 6800",
                Description = "GPU",
                Price = 600m,
                Quantity = 5,
                Category = gpuCategory
            }
        };

        await context.Categories.AddRangeAsync(new[] { cpuCategory, gpuCategory });
        await context.SaveChangesAsync();

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
} 