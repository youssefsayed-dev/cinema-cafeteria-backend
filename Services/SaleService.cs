using AutoMapper;
using BusinessManagement.Api.Data;
using BusinessManagement.Api.DTOs;
using BusinessManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessManagement.Api.Services;

public class SaleService : ISaleService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public SaleService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<SaleDto> CreateSaleAsync(CreateSaleRequest request)
    {
        if (request.Items == null || !request.Items.Any())
            throw new ArgumentException("Sale must have at least one item.");

        var sale = new Entities.Sale
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            TotalAmount = 0
        };

        decimal totalAmount = 0;

        foreach (var itemRequest in request.Items)
        {
            if (itemRequest.Quantity <= 0)
                throw new ArgumentException("Item quantity must be greater than zero.");

            var product = await _context.Products.FindAsync(itemRequest.ProductId);
            if (product == null || !product.IsActive)
                throw new KeyNotFoundException($"Product with ID {itemRequest.ProductId} not found.");

            // Check inventory availability
            var availableQuantity = await _context.InventoryBatches
                .Where(b => b.ProductId == itemRequest.ProductId)
                .SumAsync(b => b.Quantity);

            if (availableQuantity < itemRequest.Quantity)
                throw new InvalidOperationException($"Insufficient inventory for product {product.Name}. Available: {availableQuantity}, Requested: {itemRequest.Quantity}");

            var saleItem = new Entities.SaleItem
            {
                Id = Guid.NewGuid(),
                SaleId = sale.Id,
                ProductId = itemRequest.ProductId,
                Quantity = itemRequest.Quantity,
                UnitPrice = product.Price
            };

            totalAmount += saleItem.UnitPrice * saleItem.Quantity;
            _context.SaleItems.Add(saleItem);

            // Deduct inventory (FIFO - First In First Out)
            await DeductInventoryAsync(itemRequest.ProductId, itemRequest.Quantity);
        }

        sale.TotalAmount = totalAmount;
        _context.Sales.Add(sale);
        await _context.SaveChangesAsync();

        var saleDto = await _context.Sales
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .FirstOrDefaultAsync(s => s.Id == sale.Id);

        return _mapper.Map<SaleDto>(saleDto);
    }

    public async Task<List<SaleDto>> GetAllSalesAsync()
    {
        var sales = await _context.Sales
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return _mapper.Map<List<SaleDto>>(sales);
    }

    private async Task DeductInventoryAsync(Guid productId, int quantity)
    {
        var batches = await _context.InventoryBatches
            .Where(b => b.ProductId == productId && b.Quantity > 0)
            .OrderBy(b => b.CreatedAt)
            .ToListAsync();

        int remaining = quantity;

        foreach (var batch in batches)
        {
            if (remaining <= 0)
                break;

            int toDeduct = Math.Min(remaining, batch.Quantity);
            batch.Quantity -= toDeduct;
            remaining -= toDeduct;
        }

        if (remaining > 0)
            throw new InvalidOperationException("Insufficient inventory to complete the sale.");
    }
}

