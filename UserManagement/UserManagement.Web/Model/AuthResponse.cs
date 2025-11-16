namespace UserManagement.Web.Model;

public record AuthResponse(
    string Token,
    UserInfo User
);

// создать констркутор из четырех параметров
public record UserInfo(
    Guid Id,
    string Name,
    string Role
);