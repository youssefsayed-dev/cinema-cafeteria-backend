using AutoMapper;
using BusinessManagement.Api.DTOs;
using BusinessManagement.Api.Entities;

namespace BusinessManagement.Api.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Products
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductRequest, Product>();
        CreateMap<UpdateProductRequest, Product>();

        // Inventory
        CreateMap<InventoryBatch, InventoryBatchDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

        // Shifts
        CreateMap<Shift, ShiftDto>()
            .ForMember(dest => dest.StartedAt, opt => opt.MapFrom(src => src.StartedAt))
            .ForMember(dest => dest.EndedAt, opt => opt.MapFrom(src => src.EndedAt))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive));

        // Sales
        CreateMap<Sale, SaleDto>();
        CreateMap<SaleItem, SaleItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Quantity * src.UnitPrice));

        // Add more if needed
        CreateMap<InventoryBatchDto, InventoryBatch>();
    }
}