using AutoMapper;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Service.DTOs;

namespace Service.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(ProductCreateUpdateDto productDto);
    Task<ProductDto?> UpdateProductAsync(int id, ProductCreateUpdateDto productDto);
    Task<bool> DeleteProductAsync(int id);
}

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ProductService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .ToListAsync();

        var dtos = products.Select(product =>
        {
            var dto = _mapper.Map<ProductDto>(product);
            dto.Id += 1; // Adjust ID for external representation
            dto.CategoryId += 1; // Adjust CategoryId for external representation
            return dto;
        }).ToList();

        return dtos;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        // Adjust ID to match database (subtract 1 since external IDs start from 1)
        var dbId = id - 1;
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == dbId);
        
        if (product == null) return null;

        // Adjust IDs back for external representation (add 1)
        var dto = _mapper.Map<ProductDto>(product);
        dto.Id += 1;
        dto.CategoryId += 1;
        return dto;
    }

    public async Task<ProductDto> CreateProductAsync(ProductCreateUpdateDto productDto)
    {
        // Adjust CategoryId to match database (subtract 1)
        var dbCategoryId = productDto.CategoryId - 1;
        
        // Check if category exists
        var category = await _context.Categories.FindAsync(dbCategoryId);
        if (category == null)
            throw new InvalidOperationException($"Category with ID {productDto.CategoryId} not found");

        try
        {
            // Create new product
            var product = new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Quantity = productDto.Quantity,
                CategoryId = dbCategoryId
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Load the category for the response
            await _context.Entry(product).Reference(p => p.Category).LoadAsync();

            // Adjust IDs for external representation (add 1)
            var dto = _mapper.Map<ProductDto>(product);
            dto.Id += 1;
            dto.CategoryId += 1;
            return dto;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Failed to create product: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, ProductCreateUpdateDto productDto)
    {
        // Adjust IDs to match database (subtract 1)
        var dbId = id - 1;
        var dbCategoryId = productDto.CategoryId - 1;

        var product = await _context.Products.FindAsync(dbId);
        if (product == null) return null;

        var category = await _context.Categories.FindAsync(dbCategoryId);
        if (category == null)
            throw new InvalidOperationException($"Category with ID {productDto.CategoryId} not found");

        try
        {
            product.Name = productDto.Name;
            product.Description = productDto.Description;
            product.Price = productDto.Price;
            product.Quantity = productDto.Quantity;
            product.CategoryId = dbCategoryId;

            await _context.SaveChangesAsync();

            // Load the category for the response
            await _context.Entry(product).Reference(p => p.Category).LoadAsync();

            // Adjust IDs for external representation (add 1)
            var dto = _mapper.Map<ProductDto>(product);
            dto.Id += 1;
            dto.CategoryId += 1;
            return dto;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Failed to update product: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        // Adjust ID to match database (subtract 1)
        var dbId = id - 1;
        var product = await _context.Products.FindAsync(dbId);
        if (product == null) return false;

        try
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Failed to delete product: {ex.InnerException?.Message ?? ex.Message}");
        }
    }
} 