namespace Data.Entities;

public class StockImport
{
    public int Id { get; set; }
    public DateTime ImportDate { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Categories { get; set; } = string.Empty; // Stored as comma-separated values
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string Status { get; set; } = string.Empty;
} 