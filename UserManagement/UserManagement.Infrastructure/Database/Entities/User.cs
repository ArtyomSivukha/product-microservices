using UserManagement.Domain.Enums;

namespace UserManagement.Infrastructure.Database.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string Email { get; set; }
    public bool IsEmailConfirmed { get; set; } = false;
    public string Password{ get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Modified { get; set; } = DateTime.UtcNow;
}