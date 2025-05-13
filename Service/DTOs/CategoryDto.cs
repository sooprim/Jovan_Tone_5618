using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Service.DTOs;

/// <summary>
/// Example categories for computer store:
/// 1. Motherboard - Main Circuit Board
/// 2. CPU - Central Processing Unit
/// 3. GPU - Graphics Processing Unit
/// 4. RAM - Random Access Memory
/// 5. HDD - Hard Disk Drive
/// 6. SSD - Solid State Drive
/// 7. Monitor - Display Device
/// 8. Keyboard - Typing Input Device
/// 9. Mouse - Pointing Input Device
/// </summary>
public class CategoryDto
{
    /// <example>1</example>
    [JsonPropertyName("id")]
    [DefaultValue(1)]
    public int Id { get; set; }

    /// <example>Motherboard</example>
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, ErrorMessage = "Category name cannot be longer than 100 characters")]
    [DefaultValue("Motherboard")]
    public string Name { get; set; } = string.Empty;

    /// <example>Main Circuit Board</example>
    [JsonPropertyName("description")]
    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
    [DefaultValue("Main Circuit Board")]
    public string Description { get; set; } = string.Empty;

    /// <example>20</example>
    [JsonPropertyName("quantity")]
    [DefaultValue(20)]
    public int Quantity { get; set; }
} 