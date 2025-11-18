namespace UserManagement.Web.Model;

public record LoginUserRequest(
    string Email,
    string Password
);