using AutoMapper;
using ECommerce.Core.DTOs;
using ECommerce.Core.Entites.Product;

namespace ECommerce.Api.Mapping
{
    public class CategoryMapping : Profile
    {
        public CategoryMapping() 
        {
            CreateMap<CategoryDto, Category>().ReverseMap();
            CreateMap<UpdateCategoryDto,Category>().ReverseMap();   
        }
    }
}
