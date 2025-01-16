using AutoMapper;
using NewEra_Cash___Carry.Core.DTOs.category;
using NewEra_Cash___Carry.Core.DTOs.user;
using NewEra_Cash___Carry.Core.Entities;

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
        }
    }
}
