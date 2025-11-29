using Microsoft.EntityFrameworkCore;
using UserManagement.Infrastructure.Database;

namespace UserManagement.Web;

public static class DatabaseMigrator
{
    public static void MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<UserDbContext>();
            
            context.Database.Migrate();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}