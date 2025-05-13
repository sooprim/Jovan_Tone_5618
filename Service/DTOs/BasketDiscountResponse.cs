namespace Service.DTOs;

public class BasketDiscountResponse
{
    public decimal TotalBeforeDiscount { get; set; }
    public decimal TotalAfterDiscount { get; set; }
    public decimal DiscountAmount { get; set; }
    public bool DiscountApplied { get; set; }

    // Legacy properties for backward compatibility
    public decimal OriginalTotal { get; set; }
    public decimal DiscountedTotal { get; set; }
    public string DiscountDescription { get; set; } = string.Empty;
} 