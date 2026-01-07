using AutoMapper;
using BusinessManagement.Api.Data;
using BusinessManagement.Api.DTOs;
using BusinessManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessManagement.Api.Services;

public class InventoryService : IInventoryService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public InventoryService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<InventoryBatchDto> AddInventoryBatchAsync(AddInventoryBatchRequest request)
    {
        if (request.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero.");

        var product = await _context.Products.FindAsync(request.ProductId);
        if (product == null || !product.IsActive)
            throw new KeyNotFoundException("Product not found.");

        var batch = new Entities.InventoryBatch
        {
            Id = Guid.NewGuid(),
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            ExpirationDate = request.ExpirationDate,
            CreatedAt = DateTime.UtcNow
        };

        _context.InventoryBatches.Add(batch);
        await _context.SaveChangesAsync();

        var batchDto = await _context.InventoryBatches
            .Include(b => b.Product)
            .FirstOrDefaultAsync(b => b.Id == batch.Id);

        return _mapper.Map<InventoryBatchDto>(batchDto);
    }

    public async Task<List<LowStockProductDto>> GetLowStockProductsAsync(int threshold = 10)
    {
        var products = await _context.Products
            .Where(p => p.IsActive)
            .Include(p => p.InventoryBatches)
            .ToListAsync();

        var lowStockProducts = products
            .Select(p => new
            {
                Product = p,
                TotalQuantity = p.InventoryBatches.Sum(b => b.Quantity)
            })
            .Where(x => x.TotalQuantity < threshold)
            .Select(x => new LowStockProductDto
            {
                ProductId = x.Product.Id,
                ProductName = x.Product.Name,
                TotalQuantity = x.TotalQuantity,
                Threshold = threshold
            })
            .ToList();

        return lowStockProducts;
    }
}

