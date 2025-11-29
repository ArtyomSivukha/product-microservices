using Microsoft.EntityFrameworkCore;
using ProductManagement.Infrastructure.Database;

namespace ProductManagement.Web;

public static class DatabaseMigrator
{
    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ProductDbContext>();
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}