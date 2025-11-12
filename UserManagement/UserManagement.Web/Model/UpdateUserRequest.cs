namespace UserManagement.Web.Model;

public record UpdateUserRequest(
    string? FirstName,
    string? LastName
);