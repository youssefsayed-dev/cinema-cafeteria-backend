namespace BusinessManagement.Api.Entities;

public class Shift
{
    public Guid Id { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public bool IsActive { get; set; }

    // Keep navigation to Sales for totals if needed
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}

