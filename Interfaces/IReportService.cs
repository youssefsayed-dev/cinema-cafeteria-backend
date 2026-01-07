using BusinessManagement.Api.DTOs;

namespace BusinessManagement.Api.Interfaces;

public interface IReportService
{
    Task<List<DailySalesReportDto>> GetDailySalesReportAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<List<MonthlySalesReportDto>> GetMonthlySalesReportAsync(int? year = null);
}

