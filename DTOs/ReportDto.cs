namespace BusinessManagement.Api.DTOs;

public class DailySalesReportDto
{
    public DateTime Date { get; set; }
    public int TotalSales { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class MonthlySalesReportDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int TotalSales { get; set; }
    public decimal TotalRevenue { get; set; }
}

