using UserManagement.Domain.Users;

namespace UserManagement.Application.Security;

public interface ITokenService
{
    string GenerateToken(UserModel user);
    string GenerateEmailConfirmationToken(UserModel user);
}