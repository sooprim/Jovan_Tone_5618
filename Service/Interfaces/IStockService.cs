using Service.DTOs;

namespace Service.Interfaces;

public interface IStockService
{
    Task<List<StockDto>> GetAllStockAsync();
    Task<List<ProductDto>> ImportStockAsync(List<StockImportDto> stockItems);
} 