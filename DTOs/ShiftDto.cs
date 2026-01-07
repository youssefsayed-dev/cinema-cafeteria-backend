namespace BusinessManagement.Api.DTOs;

public class ShiftDto
{
    public Guid Id { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public bool IsActive { get; set; }
}

public class StartShiftRequest
{
    public decimal? OpeningCash { get; set; }
}

public class EndShiftRequest
{
    public decimal? ClosingCash { get; set; }
} 