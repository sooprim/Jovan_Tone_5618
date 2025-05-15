using AutoMapper;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MockQueryable.Moq;
using Moq;
using Service.DTOs;
using Service.Services;
using Xunit;

namespace Tests.Unit;

public class ProductUnitTests
{
    private readonly Mock<DbSet<Product>> _mockProductDbSet;
    private readonly Mock<DbSet<Category>> _mockCategoryDbSet;
    private readonly Mock<ApplicationDbContext> _mockContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductService _service;

    public ProductUnitTests()
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
        _service = new ProductService(_mockContext.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateProductAsync_ShouldCreateAndReturnProduct()
    {
        var categoryId = 1;
        var category = new Category { Id = categoryId, Name = "Test Category" };
        var productDto = new ProductCreateUpdateDto 
        { 
            Name = "Test Product", 
            Description = "Test Description",
            Price = 99.99m,
            Quantity = 10,
            CategoryId = categoryId
        };
        
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Quantity = 10,
            CategoryId = categoryId,
            Category = category
        };

        var productResponseDto = new ProductDto
        {
            Id = 1,
            Name = "Test Product",
            Description = "Test Description",
            Price = 99.99m,
            Quantity = 10,
            CategoryId = categoryId,
            CategoryName = category.Name
        };

        _mockMapper.Setup(m => m.Map<Product>(productDto)).Returns(product);
        _mockMapper.Setup(m => m.Map<ProductDto>(product)).Returns(productResponseDto);
        _mockContext.Setup(c => c.Categories.FindAsync(categoryId))
            .ReturnsAsync(category);

        var mockEntityEntry = new Mock<EntityEntry<Product>>();
        mockEntityEntry.Setup(e => e.Entity).Returns(product);

        _mockProductDbSet.Setup(d => d.Add(It.IsAny<Product>())).Returns((Product p) => 
        {
            p.Category = category;
            return mockEntityEntry.Object;
        });

        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var products = new List<Product>();
        var productsQueryable = products.AsQueryable();
        var mockProductDbSet = productsQueryable.BuildMockDbSet();
        _mockContext.Setup(c => c.Products).Returns(mockProductDbSet.Object);

        product.Category = category;

        var result = await _service.CreateProductAsync(productDto);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal(product.Name, result.Name);
        Assert.Equal(product.Price, result.Price);
        Assert.Equal(category.Name, result.CategoryName);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
} 