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

        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        // Adjust ID to match database (subtract 1 since external IDs start from 1)
        var dbId = id - 1;
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == dbId);
        
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateProductAsync(ProductCreateUpdateDto productDto)
    {
        // Map DTO to entity (this will handle ID adjustments)
        var product = _mapper.Map<Product>(productDto);
        
        // Check if category exists
        var category = await _context.Categories.FindAsync(product.CategoryId);
        if (category == null)
            throw new InvalidOperationException($"Category with ID {productDto.CategoryId} not found");

        try
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Load the category for the response
            await _context.Entry(product).Reference(p => p.Category).LoadAsync();

            return _mapper.Map<ProductDto>(product);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException($"Failed to create product: {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    public async Task<ProductDto?> UpdateProductAsync(int id, ProductCreateUpdateDto productDto)
    {
        // Adjust ID to match database (subtract 1)
        var dbId = id - 1;
        var product = await _context.Products.FindAsync(dbId);
        if (product == null) return null;

        // Map DTO to entity (this will handle ID adjustments)
        _mapper.Map(productDto, product);

        // Check if category exists
        var category = await _context.Categories.FindAsync(product.CategoryId);
        if (category == null)
            throw new InvalidOperationException($"Category with ID {productDto.CategoryId} not found");

        try
        {
            await _context.SaveChangesAsync();

            // Load the category for the response
            await _context.Entry(product).Reference(p => p.Category).LoadAsync();

            return _mapper.Map<ProductDto>(product);
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