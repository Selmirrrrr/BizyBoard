namespace BizyBoard.Web
{
    using System;
    using System.Threading.Tasks;
    using Data;
    using Data.Context;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class WebHostExtensions
    {
        public static async Task<IWebHost> MigrateDatabase(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var db = services.GetRequiredService<AppDbContext>();
                    db.Database.Migrate();

                    var seeder = services.GetRequiredService<AppDbContextSeeder>();
                    await seeder.Seed();
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<AppDbContext>>();
                    logger.LogError(ex, "An error occurred while migrating the database.");
                }
            }
            return webHost;
        }
    }
}