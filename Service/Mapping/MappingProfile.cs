using AutoMapper;
using Data.Entities;
using Service.DTOs;

namespace Service.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        CreateMap<ProductDto, Product>();
        
        CreateMap<Product, ProductCreateUpdateDto>();
        CreateMap<ProductCreateUpdateDto, Product>();

        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Products.Sum(p => p.Quantity)));
        CreateMap<CategoryDto, Category>();

        CreateMap<StockImport, StockImportHistoryDto>()
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => 
                src.Categories.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()));

        CreateMap<Product, StockDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
    }
} 