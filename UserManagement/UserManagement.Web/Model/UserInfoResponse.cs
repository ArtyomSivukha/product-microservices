namespace UserManagement.Web.Model;

public record UserInfoResponse(
    Guid Id,
    string Name,
    string Role
);