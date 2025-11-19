using AutoMapper;
using UserManagement.Domain.Enums;
using UserManagement.Domain.Users;
using UserManagement.Infrastructure.Database.Entities;

namespace UserManagement.Infrastructure.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserModel>()
            .ForMember(dest => dest.Role,
                opt => opt.MapFrom(src => src.Role.ToString()));
        CreateMap<UserModel, User>()
            .ForMember(dest => dest.Role,
                opt => opt.MapFrom(src => Enum.Parse<UserRole>(src.Role, true)));
    }
}


    
