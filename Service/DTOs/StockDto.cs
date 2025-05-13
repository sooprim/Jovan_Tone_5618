using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Service.DTOs;

public class StockDto
{
    [JsonPropertyName("productId")]
    [DefaultValue(1)]
    public int ProductId { get; set; }

    [JsonPropertyName("productName")]
    [DefaultValue("ASUS ROG STRIX B550-F")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("categoryName")]
    [DefaultValue("Motherboard")]
    public string CategoryName { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    [DefaultValue(10)]
    public int Quantity { get; set; }

    [JsonPropertyName("price")]
    [DefaultValue(179.99)]
    public decimal Price { get; set; }
} 