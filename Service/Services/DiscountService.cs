using Service.DTOs;
using Service.Interfaces;
using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Service.Services;

public class DiscountService : IDiscountService
{
    private readonly ApplicationDbContext _context;

    public DiscountService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<BasketDiscountResponse> CalculateDiscountAsync(List<BasketItemDto> basketItems)
    {
        // Validate stock levels first
        foreach (var item in basketItems)
        {
            // Adjust ID for database query (subtract 1 from external ID)
            var dbProductId = item.ProductId - 1;
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == dbProductId);

            if (product == null)
                throw new InvalidOperationException($"Product with ID {item.ProductId} not found");
            
            if (product.Quantity < item.Quantity)
                throw new InvalidOperationException($"Not enough stock for product '{product.Name}'. Available: {product.Quantity}, Requested: {item.Quantity}");
        }

        decimal originalTotal = basketItems.Sum(item => item.Price * item.Quantity);
        decimal discountedTotal = originalTotal;
        decimal totalDiscount = 0;
        var appliedDiscounts = new List<string>();

        // Group items by category
        var productCategories = await GetProductCategories(basketItems.Select(i => i.ProductId).ToList());
        var itemsByCategory = basketItems
            .Where(item => productCategories.ContainsKey(item.ProductId))
            .GroupBy(item => productCategories[item.ProductId]);

        // Apply category-based discounts
        foreach (var categoryGroup in itemsByCategory)
        {
            if (categoryGroup.Count() > 1)
            {
                // Apply 5% discount to the first item in each category
                var firstItem = categoryGroup.OrderBy(item => item.Price).First();
                decimal categoryDiscount = firstItem.Price * firstItem.Quantity * 0.05m;
                totalDiscount += categoryDiscount;
                appliedDiscounts.Add($"5% off first {categoryGroup.Key}");
            }
        }

        // Calculate final discounted total
        discountedTotal = originalTotal - totalDiscount;
        string discountDescription = appliedDiscounts.Any() 
            ? string.Join(", ", appliedDiscounts) 
            : "No discount applied";

        return new BasketDiscountResponse
        {
            OriginalTotal = originalTotal,
            DiscountedTotal = discountedTotal,
            DiscountAmount = totalDiscount,
            DiscountDescription = discountDescription
        };
    }

    private async Task<Dictionary<int, string>> GetProductCategories(List<int> productIds)
    {
        // Adjust IDs for database query (subtract 1 from external IDs)
        var dbProductIds = productIds.Select(id => id - 1).ToList();
        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => dbProductIds.Contains(p.Id))
            .ToDictionaryAsync(
                p => p.Id + 1, // Adjust ID back for external representation (add 1)
                p => p.Category.Name
            );
        return products;
    }
} 