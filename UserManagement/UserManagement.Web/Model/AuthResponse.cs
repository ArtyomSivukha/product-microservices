namespace UserManagement.Web.Model;

public record AuthResponse(
    string Token,
    UserInfoResponse User
);
