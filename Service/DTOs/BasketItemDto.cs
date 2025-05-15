using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Service.DTOs;

/// <summary>
/// Example request that will get 5% discount:
/// [
///   {
///     "productId": 3,  // AMD Ryzen 7 5800X ($299.99)
///     "quantity": 1
///   },
///   {
///     "productId": 4,  // Intel i5-12600K ($279.99)
///     "quantity": 1
///   }
/// ]
/// Both products are CPUs (same category), so first product gets 5% off
/// </summary>
public class BasketItemDto
{
    [JsonPropertyName("productId")]
    [Range(1, int.MaxValue, ErrorMessage = "Product ID must be greater than 0")]
    [DefaultValue(3)]  // AMD Ryzen 7 5800X
    public int ProductId { get; set; }

    [JsonPropertyName("quantity")]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    [DefaultValue(1)]  // Changed to 1 since we want different products, not multiple of same
    public int Quantity { get; set; }
} 