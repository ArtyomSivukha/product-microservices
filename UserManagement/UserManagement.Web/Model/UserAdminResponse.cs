namespace UserManagement.Web.Model;

public class UserAdminResponse : UserResponse
{
    public bool IsActive { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Modified { get; set; } = DateTime.UtcNow;
}