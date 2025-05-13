using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Service.DTOs;

public class CategoryDto
{
    [JsonPropertyName("id")]
    [DefaultValue(1)]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Category name is required")]
    [StringLength(100, ErrorMessage = "Category name cannot be longer than 100 characters")]
    [DefaultValue("Motherboard")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
    [DefaultValue("Main Circuit Board")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    [DefaultValue(20)]
    public int Quantity { get; set; }
} 