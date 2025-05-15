using Data.Entities;
using Service.DTOs;
using Service.Services;
using Xunit;

namespace Tests.Integration;

public class ProductIntegrationTests : TestBase
{
    private readonly ProductService _service;

    public ProductIntegrationTests()
    {
        _service = new ProductService(Context, Mapper);
    }

    [Fact]
    public async Task CreateAndUpdateProduct_ShouldWorkEndToEnd()
    {
        await ClearDatabase();

        var category = new Category { Name = "Test Category" };
        await Context.Categories.AddAsync(category);
        await Context.SaveChangesAsync();

        var createDto = new ProductCreateUpdateDto
        {
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Quantity = 10,
            CategoryId = category.Id
        };

        var created = await _service.CreateProductAsync(createDto);
        Assert.NotNull(created);
        Assert.Equal(createDto.Name, created.Name);
        Assert.Equal(createDto.Price, created.Price);

        var updateDto = new ProductCreateUpdateDto
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 149.99m,
            Quantity = 15,
            CategoryId = category.Id
        };

        var updated = await _service.UpdateProductAsync(created.Id, updateDto);
        Assert.NotNull(updated);
        Assert.Equal(updateDto.Name, updated.Name);
        Assert.Equal(updateDto.Price, updated.Price);
        Assert.Equal(updateDto.Quantity, updated.Quantity);
    }

    [Fact]
    public async Task GetAllProducts_ShouldIncludeCategoryInfo()
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
                Price = 99.99m,
                Quantity = 10,
                CategoryId = category.Id
            },
            new Product
            {
                Name = "Product 2",
                Price = 149.99m,
                Quantity = 5,
                CategoryId = category.Id
            }
        };

        await Context.Products.AddRangeAsync(products);
        await Context.SaveChangesAsync();

        var result = await _service.GetAllProductsAsync();

        Assert.Equal(2, result.Count);
        Assert.All(result, p => Assert.Equal("Test Category", p.CategoryName));
        Assert.Contains(result, p => p.Name == "Product 1");
        Assert.Contains(result, p => p.Name == "Product 2");
    }
} 