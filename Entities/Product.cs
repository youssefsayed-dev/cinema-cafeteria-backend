namespace BusinessManagement.Api.Entities;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public ICollection<InventoryBatch> InventoryBatches { get; set; } = new List<InventoryBatch>();
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}

