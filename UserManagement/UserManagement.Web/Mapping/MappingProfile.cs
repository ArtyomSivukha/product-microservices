using AutoMapper;
using UserManagement.Domain.Users;
using UserManagement.Web.Model;

namespace UserManagement.Web.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ResetPasswordDTO, UserModel>();
        CreateMap<SignUpUserRequest, UserModel>();
        CreateMap<UserModel, UserResponse>();
        CreateMap<UserModel, UserAdminResponse>();
    }
}