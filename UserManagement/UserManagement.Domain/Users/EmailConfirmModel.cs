namespace UserManagement.Domain.Users;

public class EmailConfirmModel
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public Guid UserId { get; set; }
}