using System.ComponentModel.DataAnnotations;

namespace Service.DTOs;

public class UpdateCategoryDto
{
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, ErrorMessage = "Category name cannot be longer than 100 characters")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
    public string? Description { get; set; }
} 