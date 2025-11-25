using UserManagement.Domain.Users;

namespace UserManagement.Web.Security;

public interface ITokenService
{
    string GenerateToken(UserModel user);
}