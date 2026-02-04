using AutoMapper;
using ECommerce.Core.DTOs;
using ECommerce.Core.Entites.Product;

namespace ECommerce.Api.Mapping
{
    public class ProductMapping : Profile
    {
        public ProductMapping()
        {
             CreateMap<Product,ProductDto>()
                .ForMember(x =>x.CategoryName
                , memberOptions: opt => opt .MapFrom(src => src.Category.Name))
                .ForMember(x =>x.CategoryId
                , memberOptions: opt => opt .MapFrom(src => src.Category.Id))
                .ReverseMap();    

            CreateMap<Photo,PhotoDto>().ReverseMap();

            CreateMap<AddProductDto,Product>()
                .ForMember(destinationMember: x => x.Photos, memberOptions: opt => opt.Ignore())
                .ReverseMap();

            CreateMap<UpdateProductDto,Product>()
                .ForMember(destinationMember: x => x.Photos, memberOptions: opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
  