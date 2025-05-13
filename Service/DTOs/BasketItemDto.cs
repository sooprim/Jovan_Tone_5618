using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Service.DTOs;

public class BasketItemDto
{
    [JsonPropertyName("productId")]
    [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than 0")]
    [DefaultValue(1)]
    public int ProductId { get; set; }

    [JsonPropertyName("productName")]
    [DefaultValue("Laptop")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    [DefaultValue(999.99)]
    public decimal Price { get; set; }

    [JsonPropertyName("quantity")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    [DefaultValue(2)]
    public int Quantity { get; set; }
} 