namespace Service.DTOs;

public class BasketDiscountResponse
{
    public decimal OriginalTotal { get; set; }
    public decimal DiscountedTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public string DiscountDescription { get; set; } = string.Empty;
} 