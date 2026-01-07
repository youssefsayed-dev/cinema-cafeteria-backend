using BusinessManagement.Api.DTOs;
using BusinessManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly IShiftService _shiftService;
    private readonly ILogger<ShiftsController> _logger;

    public ShiftsController(IShiftService shiftService, ILogger<ShiftsController> logger)
    {
        _shiftService = shiftService;
        _logger = logger;
    }

    [HttpPost("start")]
    public async Task<ActionResult<ShiftDto>> StartShift([FromBody] StartShiftRequest req)
    {
        try
        {
            Console.WriteLine("Starting shift with cash: " + req.OpeningCash);
            var shift = await _shiftService.StartShiftAsync(req);
            return CreatedAtAction(nameof(StartShift), new { id = shift.Id }, shift);
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine("Cant start shift: " + e.Message);
            return BadRequest(new { message = e.Message });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
    }

    [HttpPost("end")]
    public async Task<ActionResult<ShiftDto>> EndShift([FromBody] EndShiftRequest req)
    {
        try
        {
            Console.WriteLine("Ending shift with cash: " + req.ClosingCash);
            
            var current = await _shiftService.GetCurrentShiftAsync();
            if (current == null)
                return NotFound(new { message = "No active shift" });

            var endedShift = await _shiftService.EndShiftAsync(req);
            return Ok(endedShift);
        }
        catch (KeyNotFoundException e)
        {
            return NotFound(new { message = e.Message });
        }
        catch (InvalidOperationException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { message = e.Message });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error ending shift: " + ex.Message);
            return StatusCode(500, new { message = "Error" });
        }
    }

    [HttpGet("active")]
    public async Task<ActionResult<ShiftDto>> GetActiveShift()
    {
        var shift = await _shiftService.GetCurrentShiftAsync();
        if (shift == null) 
            return NotFound(new { message = "No active shift" });
        
        return Ok(shift);
    }
}