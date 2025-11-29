namespace ProductManagement.Application.Users;

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsActive { get; set; }
}