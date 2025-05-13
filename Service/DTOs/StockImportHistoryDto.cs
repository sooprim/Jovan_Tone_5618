using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Service.DTOs;

public class StockImportHistoryDto
{
    [JsonPropertyName("id")]
    [DefaultValue(1)]
    public int Id { get; set; }

    [JsonPropertyName("importDate")]
    [DefaultValue("2024-03-20T10:00:00Z")]
    public DateTime ImportDate { get; set; }

    [JsonPropertyName("productName")]
    [DefaultValue("Laptop")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("categories")]
    [DefaultValue(new[] { "Electronics", "Computers" })]
    public List<string> Categories { get; set; } = new();

    [JsonPropertyName("price")]
    [DefaultValue(999.99)]
    public decimal Price { get; set; }

    [JsonPropertyName("quantity")]
    [DefaultValue(10)]
    public int Quantity { get; set; }

    [JsonPropertyName("status")]
    [DefaultValue("Completed")]
    public string Status { get; set; } = string.Empty;
} 