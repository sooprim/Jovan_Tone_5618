using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Interfaces;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;

    public StockController(IStockService stockService)
    {
        _stockService = stockService;
    }

    /// <summary>
    /// Get current stock levels for all products
    /// </summary>
    /// <returns>List of products with their current stock levels</returns>
    /// <response code="200">Returns the list of products with stock information</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<StockDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<StockDto>>> GetStock()
    {
        return Ok(await _stockService.GetAllStockAsync());
    }

    /// <summary>
    /// Import new stock items
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/Stock/import
    ///     [
    ///         {
    ///             "name": "Intel Core i9-13900K",
    ///             "categories": ["CPU"],
    ///             "price": 475.99,
    ///             "quantity": 2
    ///         }
    ///     ]
    /// 
    /// Features:
    /// - Auto-creates categories if they don't exist
    /// - Auto-creates or updates products
    /// </remarks>
    /// <param name="stockItems">List of stock items to import</param>
    /// <returns>List of updated or created products</returns>
    /// <response code="200">Returns the list of updated/created products</response>
    /// <response code="400">If the request is invalid or import fails</response>
    [HttpPost("import")]
    [ProducesResponseType(typeof(List<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<List<ProductDto>>> ImportStock([FromBody] List<StockImportDto> stockItems)
    {
        if (!stockItems.Any())
            return BadRequest("No stock items provided");

        try
        {
            var result = await _stockService.ImportStockAsync(stockItems);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to import stock: {ex.Message}");
        }
    }
} 