using AutoMapper;
using BusinessManagement.Api.Data;
using BusinessManagement.Api.DTOs;
using BusinessManagement.Api.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessManagement.Api.Services;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ProductService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ProductDto>> GetAllProductsAsync()
    {
        var products = await _context.Products
            .Where(p => p.IsActive)
            .ToListAsync();

        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

        return product == null ? null : _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Product name is required.");

        if (request.Price <= 0)
            throw new ArgumentException("Product price must be greater than zero.");

        var product = new Entities.Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            IsActive = true
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductRequest request)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null || !product.IsActive)
            throw new KeyNotFoundException("Product not found.");

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Product name is required.");

        if (request.Price <= 0)
            throw new ArgumentException("Product price must be greater than zero.");

        product.Name = request.Name;
        product.Price = request.Price;

        await _context.SaveChangesAsync();

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<bool> DeleteProductAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null || !product.IsActive)
            return false;

        product.IsActive = false;
        await _context.SaveChangesAsync();

        return true;
    }
}

