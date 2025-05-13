using AutoMapper;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Service.DTOs;
using Service.Interfaces;

namespace Service.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CategoryService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _context.Categories
            .Include(c => c.Products)
            .ToListAsync();
        
        var dtos = categories.Select(category =>
        {
            var dto = _mapper.Map<CategoryDto>(category);
            dto.Id += 1; // Adjust ID for external representation
            dto.Quantity = category.Products.Sum(p => p.Quantity);
            return dto;
        }).ToList();
        
        return dtos;
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        // Adjust ID to match database (subtract 1 since external IDs start from 1)
        var dbId = id - 1;
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == dbId);
        
        if (category == null) return null;
        
        // Adjust ID back for external representation (add 1)
        var dto = _mapper.Map<CategoryDto>(category);
        dto.Id += 1;
        return dto;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto categoryDto)
    {
        var category = new Category
        {
            Name = categoryDto.Name,
            Description = categoryDto.Description ?? string.Empty
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        
        // Adjust ID for external representation (add 1)
        var dto = _mapper.Map<CategoryDto>(category);
        dto.Id += 1;
        return dto;
    }

    public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpdateCategoryDto categoryDto)
    {
        // Adjust ID to match database (subtract 1)
        var dbId = id - 1;
        var category = await _context.Categories.FindAsync(dbId);
        if (category == null) return null;

        category.Name = categoryDto.Name;
        category.Description = categoryDto.Description ?? string.Empty;

        await _context.SaveChangesAsync();
        
        // Adjust ID for external representation (add 1)
        var dto = _mapper.Map<CategoryDto>(category);
        dto.Id += 1;
        return dto;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        // Adjust ID to match database (subtract 1)
        var dbId = id - 1;
        var category = await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == dbId);

        if (category == null) return false;
        if (category.Products.Any())
            throw new InvalidOperationException("Cannot delete category that has products assigned to it");

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }
} 