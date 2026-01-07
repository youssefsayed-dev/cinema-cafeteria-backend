using BusinessManagement.Api.DTOs;
using BusinessManagement.Api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusinessManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _report;

    public ReportsController(IReportService reportService)
    {
        _report = reportService;
    }

    [HttpGet("daily-sales")]
    public async Task<ActionResult<List<DailySalesReportDto>>> GetDailySales(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        try
        {
            var dailyReport = await _report.GetDailySalesReportAsync(fromDate, toDate);
            return Ok(dailyReport);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error getting daily sales: " + ex.Message);
            return StatusCode(500, new { message = "Report error" });
        }
    }

    [HttpGet("monthly-sales")]
    public async Task<ActionResult<List<MonthlySalesReportDto>>> GetMonthlySales([FromQuery] int? year = null)
    {
        var monthlyData = await _report.GetMonthlySalesReportAsync(year);
        return Ok(monthlyData);
    }
}