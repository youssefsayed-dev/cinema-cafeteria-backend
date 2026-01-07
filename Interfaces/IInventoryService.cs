using BusinessManagement.Api.DTOs;

namespace BusinessManagement.Api.Interfaces;

public interface IInventoryService
{
    Task<InventoryBatchDto> AddInventoryBatchAsync(AddInventoryBatchRequest request);
    Task<List<LowStockProductDto>> GetLowStockProductsAsync(int threshold = 10);
}

