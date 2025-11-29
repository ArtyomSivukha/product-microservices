using Microsoft.EntityFrameworkCore;
using ProductManagement.Infrastructure.Database.Entities;

namespace ProductManagement.Infrastructure.Database;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<Product> Products { get; set; }
}