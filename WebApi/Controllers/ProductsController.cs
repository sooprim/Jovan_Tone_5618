using Microsoft.AspNetCore.Mvc;
using Service.DTOs;
using Service.Services;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    /// <returns>A list of all products in the catalog</returns>
    /// <response code="200">Returns the list of products</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
    {
        return Ok(await _productService.GetAllProductsAsync());
    }

    /// <summary>
    /// Get a specific product by id
    /// </summary>
    /// <param name="id">The ID of the product to retrieve</param>
    /// <returns>The requested product</returns>
    /// <response code="200">Returns the requested product</response>
    /// <response code="404">If the product is not found</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProduct(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null) return NotFound($"Product with ID {id} not found");
        return product;
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/Products
    ///     {
    ///         "name": "new name",
    ///         "description": "new desc",
    ///         "price": 99.99,
    ///         "quantity": 10,
    ///         "categoryId": 1
    ///     }
    /// </remarks>
    /// <param name="productDto">The product to create</param>
    /// <returns>The created product</returns>
    /// <response code="201">Returns the newly created product</response>
    /// <response code="400">If the product is invalid</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> CreateProduct(ProductCreateUpdateDto productDto)
    {
        try
        {
            var result = await _productService.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to create product: {ex.Message}");
        }
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    /// <param name="id">The ID of the product to update</param>
    /// <param name="productDto">The updated product data</param>
    /// <returns>The updated product</returns>
    /// <response code="200">Returns the updated product</response>
    /// <response code="404">If the product is not found</response>
    /// <response code="400">If the product is invalid</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(int id, ProductCreateUpdateDto productDto)
    {
        try
        {
            var result = await _productService.UpdateProductAsync(id, productDto);
            if (result == null) return NotFound($"Product with ID {id} not found");
            return result;
        }
        catch (Exception ex)
        {
            return BadRequest($"Failed to update product: {ex.Message}");
        }
    }

    /// <summary>
    /// Delete a specific product
    /// </summary>
    /// <param name="id">The ID of the product to delete</param>
    /// <returns>No content</returns>
    /// <response code="204">If the product was successfully deleted</response>
    /// <response code="404">If the product is not found</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);
        if (!result) return NotFound($"Product with ID {id} not found");
        return NoContent();
    }
}

public class ProductPostExample
{
    public static ProductDto Example => new()
    {
        Id = 19,
        Name = "new name",
        Description = "new desc",
        Price = 99.99m,
        Quantity = 10,
        CategoryId = 1,
        CategoryName = "Motherboard"
    };
} 