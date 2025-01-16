using AutoMapper;
using NewEra_Cash___Carry.Core.DTOs.category;
using NewEra_Cash___Carry.Core.DTOs.order;
using NewEra_Cash___Carry.Core.DTOs.product;
using NewEra_Cash___Carry.Core.DTOs.user;
using NewEra_Cash___Carry.Core.Entities;
using NewEra_Cash___Carry.DTOs.order.NewEra_Cash___Carry.DTOs.order;

namespace NewEra_Cash___Carry.Application.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(ur => ur.Role.Name).ToList()));

            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore())
                .ForMember(dest => dest.UserRoles, opt => opt.Ignore());

            // Category mappings
            CreateMap<CategoryPostDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();

            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ProductImages.Select(pi => pi.ImageUrl).ToList()));

            CreateMap<ProductPostDto, Product>()
                .ForMember(dest => dest.ProductImages, opt => opt.Ignore()) // Images are handled separately
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // Order mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            CreateMap<OrderCreateDto, Order>()
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItemCreateDto, OrderItem>();
        }
    }
}
