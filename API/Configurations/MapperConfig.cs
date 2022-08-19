using API.Dtos;
using AutoMapper;
using Core.Entities;

namespace API.Profiles;

public class MapperConfig : Profile
{
	public MapperConfig()
	{
		CreateMap<Product, ProductDto>()
			.ReverseMap();

		CreateMap<Category, CategoryDto>()
			.ReverseMap();

        CreateMap<Brand, BrandDto>()
            .ReverseMap();

		CreateMap<Product, ProductListDto>()
			.ForMember(dest => dest.Brand, origin => origin.MapFrom(origin => origin.Brand.Name))
			.ForMember(dest => dest.Category, origin => origin.MapFrom(origin => origin.Category.Name))
			.ReverseMap()
			.ForMember(origin => origin.Category, dest => dest.Ignore())
			.ForMember(origin => origin.Brand, dest => dest.Ignore());

        CreateMap<Product, ProductAddUpdateDto>()
            .ReverseMap()
            .ForMember(origin => origin.Category, dest => dest.Ignore())
            .ForMember(origin => origin.Brand, dest => dest.Ignore());

    }

}

