namespace UserManagement.Web.Model;

public record LoginUserRequest(
    string Username,
    string Password
);