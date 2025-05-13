using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Service.DTOs;

public class ProductQuantityDto
{
    [JsonPropertyName("id")]
    [DefaultValue(1)]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    [DefaultValue("Intel Core i9-9900K")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("categoryName")]
    [DefaultValue("CPU")]
    public string CategoryName { get; set; } = string.Empty;

    [JsonPropertyName("price")]
    [DefaultValue(475.99)]
    public decimal Price { get; set; }

    [JsonPropertyName("quantity")]
    [DefaultValue(2)]
    public int Quantity { get; set; }

    [JsonPropertyName("lastImported")]
    [DefaultValue("2024-03-20T10:00:00Z")]
    public DateTime? LastImported { get; set; }
} 