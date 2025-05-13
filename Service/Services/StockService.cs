using AutoMapper;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Service.DTOs;
using Service.Interfaces;

namespace Service.Services;

public class StockService : IStockService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public StockService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<StockDto>> GetAllStockAsync()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Select(p => new StockDto
            {
                ProductId = p.Id,
                ProductName = p.Name,
                CategoryName = p.Category.Name,
                Quantity = p.Quantity,
                Price = p.Price
            })
            .OrderBy(p => p.CategoryName)
            .ThenBy(p => p.ProductName)
            .ToListAsync();

        return products;
    }

    public async Task<List<ProductDto>> ImportStockAsync(List<StockImportDto> stockItems)
    {
        var updatedProducts = new List<Product>();

        foreach (var item in stockItems)
        {
            try
            {
                // Find or create categories
                var categories = new List<Category>();
                foreach (var categoryName in item.Categories)
                {
                    var trimmedName = categoryName.Trim();
                    var category = await _context.Categories
                        .FirstOrDefaultAsync(c => c.Name.ToLower() == trimmedName.ToLower());

                    if (category == null)
                    {
                        category = new Category
                        {
                            Name = trimmedName,
                            Description = $"Auto-created category for {trimmedName}"
                        };
                        _context.Categories.Add(category);
                        await _context.SaveChangesAsync();
                    }
                    categories.Add(category);
                }

                // Find or create product
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Name.ToLower() == item.Name.ToLower());

                if (product == null)
                {
                    // Create new product with the first category
                    product = new Product
                    {
                        Name = item.Name,
                        Description = $"Auto-created product from stock import",
                        Price = item.Price,
                        Quantity = item.Quantity,
                        CategoryId = categories.First().Id,
                        Category = categories.First()
                    };
                    _context.Products.Add(product);
                }
                else
                {
                    // Update existing product
                    product.Price = item.Price;
                    product.Quantity = item.Quantity;
                    product.CategoryId = categories.First().Id;
                    product.Category = categories.First();
                }

                await _context.SaveChangesAsync();
                updatedProducts.Add(product);
            }
            catch (Exception)
            {
                throw; // Re-throw to be handled by controller
            }
        }

        return _mapper.Map<List<ProductDto>>(updatedProducts);
    }
} 