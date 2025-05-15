using Service.DTOs;
using Service.Services;
using Xunit;

namespace Tests.Integration;

public class StockIntegrationTests : TestBase
{
    private readonly StockService _service;

    public StockIntegrationTests()
    {
        _service = new StockService(Context, Mapper);
    }

    [Fact]
    public async Task ImportStock_ShouldCreateNewProductsAndCategories()
    {
        await ClearDatabase();

        var stockItems = new List<StockImportDto>
        {
            new StockImportDto
            {
                Name = "Intel Core i9",
                Categories = new List<string> { "CPU", "Electronics" },
                Price = 499.99m,
                Quantity = 5
            },
            new StockImportDto
            {
                Name = "AMD Ryzen 9",
                Categories = new List<string> { "CPU", "Electronics" },
                Price = 449.99m,
                Quantity = 8
            }
        };

        var result = await _service.ImportStockAsync(stockItems);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.Name == "Intel Core i9" && p.CategoryName == "CPU");
        Assert.Contains(result, p => p.Name == "AMD Ryzen 9" && p.CategoryName == "CPU");
        Assert.All(result, p => Assert.True(p.Id > 0));
    }

    [Fact]
    public async Task ImportStock_WithExistingProducts_ShouldUpdateStock()
    {
        await ClearDatabase();

        var initialStock = new List<StockImportDto>
        {
            new StockImportDto
            {
                Name = "Intel Core i9",
                Categories = new List<string> { "CPU" },
                Price = 499.99m,
                Quantity = 5
            }
        };

        await _service.ImportStockAsync(initialStock);

        var updatedStock = new List<StockImportDto>
        {
            new StockImportDto
            {
                Name = "Intel Core i9",
                Categories = new List<string> { "CPU" },
                Price = 479.99m,
                Quantity = 10
            }
        };

        var result = await _service.ImportStockAsync(updatedStock);

        Assert.Single(result);
        var updatedProduct = result[0];
        Assert.Equal("Intel Core i9", updatedProduct.Name);
        Assert.Equal(479.99m, updatedProduct.Price);
        Assert.Equal(10, updatedProduct.Quantity);
    }
} 