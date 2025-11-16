using UserManagement.Domain.Users;

namespace UserManagement.Web.Security;

public interface ITokenService
{
    public string GenerateToken(UserModel user);
}