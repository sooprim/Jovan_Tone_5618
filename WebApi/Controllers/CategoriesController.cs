using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
    {
        return Ok(await _categoryService.GetAllCategoriesAsync());
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound($"Category with ID {id} not found");
        return category;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto categoryDto)
    {
        try
        {
            var result = await _categoryService.CreateCategoryAsync(categoryDto);
            return CreatedAtAction(nameof(GetCategory), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to create category: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, UpdateCategoryDto categoryDto)
    {
        try
        {
            var result = await _categoryService.UpdateCategoryAsync(id, categoryDto);
            if (result == null) return NotFound($"Category with ID {id} not found");
            return result;
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to update category: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        try
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result) return NotFound($"Category with ID {id} not found");
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to delete category: {ex.Message}");
        }
    }
} 