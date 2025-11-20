using AutoMapper;
using UserManagement.Domain.Users;
using UserManagement.Web.Model;

namespace UserManagement.Web.Mapping;

public class MappingResetPassword : Profile
{
    public MappingResetPassword()
    {
        CreateMap<ResetPasswordDTO, UserModel>();
    }
}