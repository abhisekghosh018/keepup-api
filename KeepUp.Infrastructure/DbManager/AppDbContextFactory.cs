using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace KeepUp.Infrastructure.DbManager
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {

            var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Development";

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException(
                    "Design-time connection string 'DefaultConnection' is missing.");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseNpgsql(connectionString)
                .Options;

            return new AppDbContext(options);


            #region Having design-time issues with EF Core tools?
            //// Build configuration ONLY from environment variables
            //// (works in Docker, CI, local)
            //var configuration = new ConfigurationBuilder()
            //    .AddEnvironmentVariables()
            //    .Build();

            //// Prefer standard ConnectionStrings, fallback to env-style
            //var connectionString =
            //    configuration.GetConnectionString("DefaultConnection")
            //    ?? configuration["ConnectionStrings__DefaultConnection"];

            //if (string.IsNullOrWhiteSpace(connectionString))
            //    throw new InvalidOperationException(
            //        "Design-time connection string 'DefaultConnection' is missing.");

            //var options = new DbContextOptionsBuilder<AppDbContext>()
            //    .UseNpgsql(connectionString)
            //    .Options;

            //return new AppDbContext(options);
            #endregion
        }
    }
}
