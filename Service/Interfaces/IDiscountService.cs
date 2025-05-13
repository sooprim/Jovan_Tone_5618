using Service.DTOs;

namespace Service.Interfaces;

public interface IDiscountService
{
    Task<BasketDiscountResponse> CalculateBasketDiscount(List<BasketItemDto> basketItems);
    Task<BasketDiscountResponse> CalculateDiscountAsync(List<BasketItemDto> basketItems);
} 