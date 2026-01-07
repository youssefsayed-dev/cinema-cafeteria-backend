using BusinessManagement.Api.DTOs;

namespace BusinessManagement.Api.Interfaces;

public interface IShiftService
{
    Task<ShiftDto> StartShiftAsync(StartShiftRequest request);
    Task<ShiftDto> EndShiftAsync(EndShiftRequest request);
    Task<ShiftDto?> GetCurrentShiftAsync();
}

