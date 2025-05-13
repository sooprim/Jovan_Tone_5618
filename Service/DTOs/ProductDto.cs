using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Service.DTOs;

public class ProductCreateUpdateDto
{
    /// <example>ASUS ROG Maximus Z790 Hero</example>
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, ErrorMessage = "Product name cannot be longer than 100 characters")]
    [DefaultValue("ASUS ROG Maximus Z790 Hero")]
    public string Name { get; set; } = string.Empty;

    /// <example>High-end Z790 motherboard with PCIe 5.0 and DDR5 support</example>
    [JsonPropertyName("description")]
    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
    [DefaultValue("High-end Z790 motherboard with PCIe 5.0 and DDR5 support")]
    public string Description { get; set; } = string.Empty;

    /// <example>699.99</example>
    [JsonPropertyName("price")]
    [Range(0.01, 99999.99, ErrorMessage = "Price must be between 0.01 and 99,999.99")]
    [DefaultValue(699.99)]
    public decimal Price { get; set; }

    /// <example>10</example>
    [JsonPropertyName("quantity")]
    [Range(0, 10000, ErrorMessage = "Quantity must be between 0 and 10,000")]
    [DefaultValue(10)]
    public int Quantity { get; set; }

    /// <example>1</example>
    [JsonPropertyName("categoryId")]
    [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0")]
    [DefaultValue(1)] // Maps to Motherboard category
    public int CategoryId { get; set; }
}

/// <summary>
/// Example products matching actual database categories:
/// 1. Motherboard - ASUS ROG Maximus Z790 Hero
/// 2. CPU - Intel Core i9-13900K
/// 3. GPU - NVIDIA RTX 4090
/// 4. RAM - Corsair Vengeance 32GB DDR5
/// 5. HDD - Seagate Barracuda 4TB
/// 6. SSD - Samsung 990 PRO 2TB
/// 7. Monitor - LG 27GP950 4K Gaming
/// 8. Keyboard - Razer BlackWidow V4 Pro
/// 9. Mouse - Logitech G Pro X Superlight 2
/// </summary>
public class ProductDto : ProductCreateUpdateDto
{
    /// <example>1</example>
    [JsonPropertyName("id")]
    [DefaultValue(1)]
    public int Id { get; set; }

    /// <example>Motherboard</example>
    [JsonPropertyName("categoryName")]
    [DefaultValue("Motherboard")]
    public string CategoryName { get; set; } = string.Empty;
} 