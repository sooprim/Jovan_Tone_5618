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
        var dbId = id - 1;
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == dbId);
        
        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateProductAsync(ProductCreateUpdateDto productDto)
    {
        var product = _mapper.Map<Product>(productDto);
        
        var category = await _context.Categories.FindAsync(product.CategoryId);
        if (category == null)
            throw new InvalidOperationException($"Category with ID {productDto.CategoryId} not found");

        try
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

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
        var dbId = id - 1;
        var product = await _context.Products.FindAsync(dbId);
        if (product == null) return null;

        _mapper.Map(productDto, product);

        var category = await _context.Categories.FindAsync(product.CategoryId);
        if (category == null)
            throw new InvalidOperationException($"Category with ID {productDto.CategoryId} not found");

        try
        {
            await _context.SaveChangesAsync();

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