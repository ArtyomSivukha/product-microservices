using UserManagement.Application;

namespace UserManagement.Web.Security;

public class CurrentUserAccessor : ICurrentUserAccessor
{
    public Guid UserId { get; set; }
    public string UserToken { get; set; }
}