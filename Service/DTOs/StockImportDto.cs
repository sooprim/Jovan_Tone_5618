using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Service.DTOs;

public class StockImportDto
{
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, ErrorMessage = "Product name cannot be longer than 100 characters")]
    [JsonPropertyName("name")]
    [DefaultValue("Intel Core i9-9900K")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "At least one category is required")]
    [MinLength(1, ErrorMessage = "At least one category must be specified")]
    [JsonPropertyName("categories")]
    [DefaultValue(new[] { "CPU", "Processors" })]
    public List<string> Categories { get; set; } = new();

    [Required(ErrorMessage = "Price is required")]
    [Range(0.01, 99999.99, ErrorMessage = "Price must be between 0.01 and 99,999.99")]
    [JsonPropertyName("price")]
    [DefaultValue(475.99)]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, 10000, ErrorMessage = "Quantity must be between 0 and 10,000")]
    [JsonPropertyName("quantity")]
    [DefaultValue(2)]
    public int Quantity { get; set; }
} 