using AutoMapper;
using BusinessManagement.Api.Data;
using BusinessManagement.Api.DTOs;
using BusinessManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessManagement.Api.Services;

public class ShiftService : IShiftService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ShiftService> _logger;

    public ShiftService(AppDbContext context, IMapper mapper, ILogger<ShiftService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    // Start a global active shift
    public async Task<ShiftDto> StartShiftAsync(StartShiftRequest request)
    {
        // TEMP log
        _logger.LogInformation("[StartShift] Incoming request: {@Request}", request);

        // Check if any active shift exists globally
        var activeShift = await _context.Shifts.FirstOrDefaultAsync(s => s.EndedAt == null || s.IsActive);
        _logger.LogInformation("[StartShift] Existing active shift: {@Active}", activeShift == null ? "<none>" : (object)activeShift.Id);

        if (activeShift != null)
            throw new InvalidOperationException("Active shift already exists.");

        var shift = new Entities.Shift
        {
            Id = Guid.NewGuid(),
            StartedAt = DateTime.UtcNow,
            EndedAt = null,
            IsActive = true
        };

        _context.Shifts.Add(shift);
        await _context.SaveChangesAsync();

        var dto = _mapper.Map<ShiftDto>(shift);
        return dto;
    }

    // End the current active shift
    public async Task<ShiftDto> EndShiftAsync(EndShiftRequest request)
    {
        _logger.LogInformation("[EndShift] Incoming request: {@Request}", request);

        var activeShift = await _context.Shifts.FirstOrDefaultAsync(s => s.EndedAt == null || s.IsActive);
        _logger.LogInformation("[EndShift] Found active shift: {@Active}", activeShift == null ? "<none>" : (object)activeShift.Id);

        if (activeShift == null)
            throw new KeyNotFoundException("No active shift found.");

        activeShift.EndedAt = DateTime.UtcNow;
        activeShift.IsActive = false;

        await _context.SaveChangesAsync();

        var dto = _mapper.Map<ShiftDto>(activeShift);
        return dto;
    }

    // Return the current active shift or null
    public async Task<ShiftDto?> GetCurrentShiftAsync()
    {
        var shift = await _context.Shifts.FirstOrDefaultAsync(s => s.EndedAt == null || s.IsActive);
        if (shift == null) return null;
        var dto = _mapper.Map<ShiftDto>(shift);
        return dto;
    }
}

