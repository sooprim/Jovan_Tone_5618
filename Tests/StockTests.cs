using AutoMapper;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Service.DTOs;
using Service.Mapping;
using Service.Services;
using Xunit;

namespace Tests;

public class StockTests
{
    private readonly IMapper _mapper;
    private readonly string _dbName;

    public StockTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MappingProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _dbName = $"TestDb_Stock_{Guid.NewGuid()}";
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
    public async Task ImportStockAsync_WithNewProduct_ShouldCreateProductAndCategory()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await ClearDatabase(context);
        
        var service = new StockService(context, _mapper);
        var stockItems = new List<StockImportDto>
        {
            new StockImportDto
            {
                Name = "Intel Core i9-9900K",
                Categories = new List<string> { "CPU" },
                Price = 475.99m,
                Quantity = 2
            }
        };

        // Act
        var result = await service.ImportStockAsync(stockItems);

        // Assert
        Assert.Single(result);
        Assert.Equal("Intel Core i9-9900K", result[0].Name);
        Assert.Equal(475.99m, result[0].Price);
        Assert.Equal(2, result[0].Quantity);
        Assert.Equal("CPU", result[0].CategoryName);

        // Verify database state
        var dbProduct = await context.Products.Include(p => p.Category).FirstAsync();
        Assert.Equal("Intel Core i9-9900K", dbProduct.Name);
        Assert.Equal("CPU", dbProduct.Category.Name);
    }

    [Fact]
    public async Task ImportStockAsync_WithExistingProduct_ShouldUpdateProduct()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await ClearDatabase(context);

        var category = new Category { Name = "CPU" };
        var product = new Product
        {
            Name = "Intel Core i9-9900K",
            Description = "Original Description",
            Price = 450.99m,
            Quantity = 1,
            Category = category
        };
        context.Categories.Add(category);
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var service = new StockService(context, _mapper);
        var stockItems = new List<StockImportDto>
        {
            new StockImportDto
            {
                Name = "Intel Core i9-9900K",
                Categories = new List<string> { "CPU" },
                Price = 475.99m,
                Quantity = 2
            }
        };

        // Act
        var result = await service.ImportStockAsync(stockItems);

        // Assert
        Assert.Single(result);
        Assert.Equal("Intel Core i9-9900K", result[0].Name);
        Assert.Equal(475.99m, result[0].Price);
        Assert.Equal(2, result[0].Quantity);
    }

    [Fact]
    public async Task ImportStockAsync_WithMultipleCategories_ShouldUseFirstCategory()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await ClearDatabase(context);

        var service = new StockService(context, _mapper);
        var stockItems = new List<StockImportDto>
        {
            new StockImportDto
            {
                Name = "Gaming Keyboard",
                Categories = new List<string> { "Keyboard", "Gaming", "Peripherals" },
                Price = 89.99m,
                Quantity = 10
            }
        };

        // Act
        var result = await service.ImportStockAsync(stockItems);

        // Assert
        Assert.Single(result);
        Assert.Equal("Keyboard", result[0].CategoryName);
        
        // Verify all categories were created
        var categories = await context.Categories.Select(c => c.Name).ToListAsync();
        Assert.Contains("Keyboard", categories);
        Assert.Contains("Gaming", categories);
        Assert.Contains("Peripherals", categories);
    }

    [Fact]
    public async Task ImportStockAsync_WithMultipleProducts_ShouldImportAll()
    {
        // Arrange
        using var context = new ApplicationDbContext(GetDbContextOptions());
        await ClearDatabase(context);

        var service = new StockService(context, _mapper);
        var stockItems = new List<StockImportDto>
        {
            new StockImportDto
            {
                Name = "Intel Core i9-9900K",
                Categories = new List<string> { "CPU" },
                Price = 475.99m,
                Quantity = 2
            },
            new StockImportDto
            {
                Name = "Razer BlackWidow",
                Categories = new List<string> { "Keyboard" },
                Price = 89.99m,
                Quantity = 10
            }
        };

        // Act
        var result = await service.ImportStockAsync(stockItems);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "Intel Core i9-9900K" && p.CategoryName == "CPU");
        Assert.Contains(result, p => p.Name == "Razer BlackWidow" && p.CategoryName == "Keyboard");
    }
} 