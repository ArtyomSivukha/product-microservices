using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Infrastructure.Database.Entities;

public class EmailConfirm
{
    public Guid Id { get; set; }
    public string Token { get; set; }
    public Guid UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
}