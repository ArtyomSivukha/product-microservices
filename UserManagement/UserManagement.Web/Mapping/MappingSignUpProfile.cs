using AutoMapper;
using UserManagement.Domain.Users;
using UserManagement.Web.Model;

namespace UserManagement.Web.Mapping;

public class MappingSignUpProfile : Profile
{
    public MappingSignUpProfile()
    {
        CreateMap<SignUpUserRequest, UserModel>();
    }
}