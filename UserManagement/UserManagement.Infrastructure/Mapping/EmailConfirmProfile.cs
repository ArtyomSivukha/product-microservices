using AutoMapper;
using UserManagement.Domain.Users;
using UserManagement.Infrastructure.Database.Entities;

namespace UserManagement.Infrastructure.Mapping;

public class EmailConfirmProfile : Profile
{
    public EmailConfirmProfile()
    {
        CreateMap<EmailConfirm, EmailConfirmModel>();
        CreateMap<EmailConfirmModel, EmailConfirm>();
    }
}