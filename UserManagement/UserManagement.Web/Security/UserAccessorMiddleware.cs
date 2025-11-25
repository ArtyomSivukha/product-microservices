using System.Security.Claims;
using UserManagement.Application;

namespace UserManagement.Web.Security;

public class UserAccessorMiddleware
{
    private readonly RequestDelegate next;

    public UserAccessorMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentUserAccessor accessor)
    {
        var token = context
            .Request.Headers["Authorization"]
            .ToString()
            .Replace("Bearer ", string.Empty, StringComparison.OrdinalIgnoreCase);
        var userId = GetUserIdFromContext(context);
        
        (accessor as CurrentUserAccessor)!.UserId = userId;
        (accessor as CurrentUserAccessor)!.UserToken = token;
        
        await next(context);
    }
    
    private Guid GetUserIdFromContext(HttpContext context)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userIdClaim);
    }
}