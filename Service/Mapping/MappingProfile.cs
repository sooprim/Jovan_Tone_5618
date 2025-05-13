using AutoMapper;
using Data.Entities;
using Service.DTOs;

namespace Service.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id + 1))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId + 1));
        
        CreateMap<ProductDto, Product>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id - 1))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId - 1));
        
        CreateMap<Product, ProductCreateUpdateDto>();
        CreateMap<ProductCreateUpdateDto, Product>()
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId - 1));

        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Products.Sum(p => p.Quantity)))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id + 1));
        
        CreateMap<CategoryDto, Category>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id - 1));
        
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();

        CreateMap<StockImport, StockImportHistoryDto>()
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => 
                src.Categories.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()));

        CreateMap<Product, StockDto>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id + 1))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
    }
} 