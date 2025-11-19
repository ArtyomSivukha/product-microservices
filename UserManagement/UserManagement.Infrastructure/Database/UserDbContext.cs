using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Database.Entities;

namespace UserManagement.Infrastructure.Database;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<EmailConfirm> EmailConfirmationTokens { get; set; }
}