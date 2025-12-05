using Application.DTO.Categories;
using Application.DTO.Products;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<CreateProductDto, Product>().ReverseMap();
        CreateMap<UpdateProductDto, Product>()
            .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != string.Empty))
            .ForMember(dest => dest.Price, opt => opt.Condition(src => src.Price != 0))
            .ForMember(dest => dest.CategoryId, opt => opt.Condition(src => src.CategoryId != 0));
        CreateMap<Product, ProductResponseDto>();
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<Category, CategoryResponseDto>();
    }
}