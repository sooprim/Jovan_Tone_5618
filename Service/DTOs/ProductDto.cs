using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Service.DTOs;

public class ProductCreateUpdateDto
{
    [JsonPropertyName("name")]
    [Required(ErrorMessage = "Product name is required")]
    [StringLength(100, ErrorMessage = "Product name cannot be longer than 100 characters")]
    [DefaultValue("ASUS ROG Maximus Z790 Hero")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
    [DefaultValue("High-end Z790 motherboard with PCIe 5.0 and DDR5 support")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    [Range(0.01, 99999.99, ErrorMessage = "Price must be between 0.01 and 99,999.99")]
    [DefaultValue(699.99)]
    public decimal Price { get; set; }

    [JsonPropertyName("quantity")]
    [Range(0, 10000, ErrorMessage = "Quantity must be between 0 and 10,000")]
    [DefaultValue(10)]
    public int Quantity { get; set; }

    [JsonPropertyName("categoryId")]
    [Range(1, int.MaxValue, ErrorMessage = "Category ID must be greater than 0")]
    [DefaultValue(1)]
    public int CategoryId { get; set; }
}

public class ProductDto : ProductCreateUpdateDto
{
    [JsonPropertyName("id")]
    [DefaultValue(1)]
    public int Id { get; set; }

    [JsonPropertyName("categoryName")]
    [DefaultValue("Motherboard")]
    public string CategoryName { get; set; } = string.Empty;
}