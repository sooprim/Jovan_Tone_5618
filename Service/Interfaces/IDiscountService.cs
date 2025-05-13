using Service.DTOs;

namespace Service.Interfaces;

public interface IDiscountService
{
    Task<BasketDiscountResponse> CalculateDiscountAsync(List<BasketItemDto> basketItems);
} 