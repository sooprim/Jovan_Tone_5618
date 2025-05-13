using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;

namespace WebApi.Controllers;

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