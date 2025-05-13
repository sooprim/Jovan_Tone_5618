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

    public DiscountService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<BasketDiscountResponse> CalculateBasketDiscount(List<BasketItemDto> basketItems)
    {
        decimal totalBeforeDiscount = 0;

        foreach (var item in basketItems)
        {
            var dbProductId = item.ProductId - 1;
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == dbProductId);

            if (product == null)
                throw new InvalidOperationException($"Product with ID {item.ProductId} not found");
            
            if (product.Quantity < item.Quantity)
                throw new InvalidOperationException($"Not enough stock for product '{product.Name}'. Available: {product.Quantity}, Requested: {item.Quantity}");

            totalBeforeDiscount += product.Price * item.Quantity;
        }

        decimal discountAmount = totalBeforeDiscount * 0.10m;
        decimal totalAfterDiscount = totalBeforeDiscount - discountAmount;

        return new BasketDiscountResponse
        {
            TotalBeforeDiscount = totalBeforeDiscount,
            TotalAfterDiscount = totalAfterDiscount,
            DiscountAmount = discountAmount,
            DiscountApplied = true
        };
    }

    public async Task<BasketDiscountResponse> CalculateDiscountAsync(List<BasketItemDto> basketItems)
    {
        var result = await CalculateBasketDiscount(basketItems);
        return new BasketDiscountResponse
        {
            OriginalTotal = result.TotalBeforeDiscount,
            DiscountedTotal = result.TotalAfterDiscount,
            DiscountAmount = result.DiscountAmount,
            DiscountDescription = result.DiscountApplied ? "10% discount applied" : "No discount applied"
        };
    }

    private async Task<Dictionary<int, string>> GetProductCategories(List<int> productIds)
    {
        var dbProductIds = productIds.Select(id => id - 1).ToList();
        var products = await _context.Products
            .Include(p => p.Category)
            .Where(p => dbProductIds.Contains(p.Id))
            .ToDictionaryAsync(
                p => p.Id + 1,
                p => p.Category.Name
            );
        return products;
    }
} 