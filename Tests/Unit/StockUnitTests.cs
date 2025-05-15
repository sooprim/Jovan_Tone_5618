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

public class StockUnitTests
{
    private readonly Mock<DbSet<Product>> _mockProductDbSet;
    private readonly Mock<DbSet<Category>> _mockCategoryDbSet;
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly StockService _service;

    public StockUnitTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _mockProductDbSet = new Mock<DbSet<Product>>();
        _mockCategoryDbSet = new Mock<DbSet<Category>>();
        _mockContext = new Mock<ApplicationDbContext>(options);
        _mockMapper = new Mock<IMapper>();
        _mockContext.Setup(c => c.Products).Returns(_mockProductDbSet.Object);
        _mockContext.Setup(c => c.Categories).Returns(_mockCategoryDbSet.Object);
        _service = new StockService(_mockContext.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task ImportStockAsync_WithNewProduct_ShouldCreateProductAndCategory()
    {
        var stockItem = new StockImportDto
        {
            Name = "Test Product",
            Categories = new List<string> { "Test Category" },
            Price = 99.99m,
            Quantity = 10
        };

        var category = new Category { Id = 1, Name = "Test Category" };
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            Price = 99.99m,
            Quantity = 10,
            CategoryId = 1,
            Category = category
        };

        var productDto = new ProductDto
        {
            Id = 1,
            Name = "Test Product",
            Price = 99.99m,
            Quantity = 10,
            CategoryId = 1,
            CategoryName = "Test Category"
        };

        var products = new List<Product>();
        var categories = new List<Category>();

        var categoriesQueryable = categories.AsQueryable();
        var mockCategoryDbSet = categoriesQueryable.BuildMockDbSet();
        _mockContext.Setup(c => c.Categories).Returns(mockCategoryDbSet.Object);

        var productsQueryable = products.AsQueryable();
        var mockProductDbSet = productsQueryable.BuildMockDbSet();
        _mockContext.Setup(c => c.Products).Returns(mockProductDbSet.Object);

        mockProductDbSet.Setup(d => d.Add(It.IsAny<Product>())).Callback<Product>(p => 
        {
            p.Category = category;
            products.Add(p);
        });
        mockCategoryDbSet.Setup(d => d.Add(It.IsAny<Category>())).Callback<Category>(c => categories.Add(c));

        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _mockContext.Setup(c => c.Entry(It.IsAny<Product>()))
            .Callback<Product>(p => p.Category = category);

        _mockMapper.Setup(m => m.Map<List<ProductDto>>(It.IsAny<List<Product>>()))
            .Returns((List<Product> source) => new List<ProductDto> { productDto });

        var result = await _service.ImportStockAsync(new List<StockImportDto> { stockItem });

        Assert.Single(result);
        Assert.Equal(stockItem.Name, result[0].Name);
        Assert.Equal(stockItem.Price, result[0].Price);
        Assert.Equal(stockItem.Quantity, result[0].Quantity);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
} 