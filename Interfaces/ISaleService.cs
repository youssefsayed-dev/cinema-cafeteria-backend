using BusinessManagement.Api.DTOs;

namespace BusinessManagement.Api.Interfaces;

public interface ISaleService
{
    Task<SaleDto> CreateSaleAsync(CreateSaleRequest request);
    Task<List<SaleDto>> GetAllSalesAsync();
}

