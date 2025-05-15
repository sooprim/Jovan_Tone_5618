using Data.Entities;
using Service.DTOs;
using Service.Services;
using Xunit;

namespace Tests.Integration;

public class CategoryIntegrationTests : TestBase
{
    private readonly CategoryService _service;

    public CategoryIntegrationTests()
    {
        _service = new CategoryService(Context, Mapper);
    }

    [Fact]
    public async Task CreateAndGetCategory_ShouldWorkEndToEnd()
    {
        await ClearDatabase();

        var createDto = new CreateCategoryDto
        {
            Name = "Test Category",
            Description = "Test Description"
        };

        var created = await _service.CreateCategoryAsync(createDto);
        Assert.NotNull(created);
        Assert.Equal(createDto.Name, created.Name);

        var retrieved = await _service.GetCategoryByIdAsync(created.Id);
        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal(created.Name, retrieved.Name);
    }

    [Fact]
    public async Task DeleteCategory_WithProducts_ShouldThrowException()
    {
        await ClearDatabase();

        var category = new Category { Name = "Test Category" };
        await Context.Categories.AddAsync(category);
        await Context.SaveChangesAsync();

        var product = new Product
        {
            Name = "Test Product",
            CategoryId = category.Id
        };
        await Context.Products.AddAsync(product);
        await Context.SaveChangesAsync();

        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.DeleteCategoryAsync(category.Id));
    }
} 