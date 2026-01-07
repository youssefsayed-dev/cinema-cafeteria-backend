using BusinessManagement.Api.Data;
using BusinessManagement.Api.DTOs;
using BusinessManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessManagement.Api.Services;

public class ReportService : IReportService
{
    private readonly AppDbContext _context;

    public ReportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DailySalesReportDto>> GetDailySalesReportAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.UtcNow.AddDays(-30);
        var end = endDate ?? DateTime.UtcNow;

        var sales = await _context.Sales
            .Where(s => s.CreatedAt >= start && s.CreatedAt <= end)
            .ToListAsync();

        var dailyReport = sales
            .GroupBy(s => s.CreatedAt.Date)
            .Select(g => new DailySalesReportDto
            {
                Date = g.Key,
                TotalSales = g.Count(),
                TotalRevenue = g.Sum(s => s.TotalAmount)
            })
            .OrderBy(r => r.Date)
            .ToList();

        return dailyReport;
    }

    public async Task<List<MonthlySalesReportDto>> GetMonthlySalesReportAsync(int? year = null)
    {
        var targetYear = year ?? DateTime.UtcNow.Year;

        var sales = await _context.Sales
            .Where(s => s.CreatedAt.Year == targetYear)
            .ToListAsync();

        var monthlyReport = sales
            .GroupBy(s => new { s.CreatedAt.Year, s.CreatedAt.Month })
            .Select(g => new MonthlySalesReportDto
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalSales = g.Count(),
                TotalRevenue = g.Sum(s => s.TotalAmount)
            })
            .OrderBy(r => r.Year)
            .ThenBy(r => r.Month)
            .ToList();

        return monthlyReport;
    }
}

