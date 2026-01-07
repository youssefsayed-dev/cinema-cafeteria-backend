namespace BusinessManagement.Api.DTOs;

public class InventoryBatchDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime ExpirationDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AddInventoryBatchRequest
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpirationDate { get; set; }
}

public class LowStockProductDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
    public int Threshold { get; set; } = 10;
}

