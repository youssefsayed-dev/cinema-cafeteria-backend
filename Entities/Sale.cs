namespace BusinessManagement.Api.Entities;

public class Sale
{
    public Guid Id { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}

