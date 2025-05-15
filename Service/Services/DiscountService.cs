using AutoMapper;
using Service.DTOs;
using Service.Interfaces;
using Data.Context;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Service.Services;

public class DiscountService : IDiscountService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private const decimal CATEGORY_DISCOUNT_RATE = 0.05m; // 5% discount

    public DiscountService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BasketDiscountResponse> CalculateBasketDiscount(List<BasketItemDto> basketItems)
    {
        decimal totalBeforeDiscount = 0;
        decimal totalDiscount = 0;

        // Group items by category to check for multiple items in same category
        var productsByCategory = new Dictionary<int, List<(decimal Price, int Quantity)>>();
        
        // First pass: Validate products and collect category information
        foreach (var item in basketItems)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == item.ProductId);

            if (product == null)
                throw new InvalidOperationException($"Product with ID {item.ProductId} not found");
            
            if (product.Quantity < item.Quantity)
                throw new InvalidOperationException(
                    $"Not enough stock for product '{product.Name}'. Available: {product.Quantity}, Requested: {item.Quantity}");

            // Add to category group
            if (!productsByCategory.ContainsKey(product.CategoryId))
                productsByCategory[product.CategoryId] = new List<(decimal Price, int Quantity)>();
            
            productsByCategory[product.CategoryId].Add((product.Price, item.Quantity));
            
            // Add to total before discount
            totalBeforeDiscount += product.Price * item.Quantity;
        }

        // Second pass: Calculate discounts
        foreach (var categoryGroup in productsByCategory)
        {
            var productsInCategory = categoryGroup.Value;
            
            // If there's more than one product in this category
            if (productsInCategory.Count > 1)
            {
                // Apply discount only to the first product
                var firstProduct = productsInCategory[0];
                decimal discountAmount = firstProduct.Price * firstProduct.Quantity * CATEGORY_DISCOUNT_RATE;
                totalDiscount += discountAmount;
            }
        }

        decimal totalAfterDiscount = totalBeforeDiscount - totalDiscount;
        bool discountApplied = totalDiscount > 0;

        return new BasketDiscountResponse
        {
            TotalBeforeDiscount = totalBeforeDiscount,
            TotalAfterDiscount = totalAfterDiscount,
            DiscountAmount = totalDiscount,
            DiscountApplied = discountApplied,
            OriginalTotal = totalBeforeDiscount,
            DiscountedTotal = totalAfterDiscount,
            DiscountDescription = discountApplied 
                ? "5% discount applied to first product in categories with multiple items" 
                : "No discount applied"
        };
    }

    public async Task<BasketDiscountResponse> CalculateDiscountAsync(List<BasketItemDto> basketItems)
    {
        return await CalculateBasketDiscount(basketItems);
    }

    private async Task<Dictionary<int, string>> GetProductCategories(List<int> productIds)
    {
        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => productIds.Contains(p.Id))
            .ToDictionaryAsync(
                p => p.Id,
                p => p.Category.Name
            );
        return products;
    }
} 