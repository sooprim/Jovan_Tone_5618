using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;
using System.ComponentModel;

namespace WebApi.Controllers;

/// <summary>
/// Controller for calculating discounts on shopping baskets
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DiscountController : ControllerBase
{
    private readonly IDiscountService _discountService;

    public DiscountController(IDiscountService discountService)
    {
        _discountService = discountService;
    }

    /// <summary>
    /// Calculate discount for items in the basket
    /// </summary>
    /// <remarks>
    /// Example that will get a 5% discount:
    /// 
    ///     POST /api/Discount/calculate
    ///     [
    ///       {
    ///         "productId": 3,  // AMD Ryzen 7 5800X CPU ($299.99)
    ///         "quantity": 2    // Buying 2 units
    ///       },
    ///       {
    ///         "productId": 4,  // Intel i5-12600K CPU ($279.99)
    ///         "quantity": 1    // Buying 1 unit
    ///       }
    ///     ]
    /// 
    /// This will get a 5% discount on the first CPU because:
    /// 1. There are multiple products in the same category (both are CPUs)
    /// 2. The 5% discount applies only to the first product (AMD Ryzen)
    /// 3. Discount calculation: $299.99 × 5% = $15 discount
    /// 
    /// Total calculation:
    /// - AMD Ryzen: $299.99 × 2 = $599.98
    /// - Intel i5: $279.99 × 1 = $279.99
    /// - Subtotal: $879.97
    /// - Discount: $15 (5% off first AMD Ryzen only)
    /// - Final total: $864.97
    /// 
    /// Note: The discount ONLY applies when you have different products in the same category.
    /// Buying multiple quantities of the same product will NOT trigger the discount.
    /// </remarks>
    /// <param name="basketItems">List of items in the basket</param>
    /// <returns>Discount calculation result</returns>
    [HttpPost("calculate")]
    [ProducesResponseType(typeof(BasketDiscountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BasketDiscountResponse>> CalculateDiscount([FromBody] List<BasketItemDto> basketItems)
    {
        if (!basketItems.Any())
            return BadRequest("Basket cannot be empty");

        return await _discountService.CalculateDiscountAsync(basketItems);
    }
} 