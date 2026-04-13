using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TunNetCom.SilkRoadErp.Sales.Domain.Entites;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SalesContext>
{
    public SalesContext CreateDbContext(string[] args)
    {
        // Configuration pour le design-time (création de migrations)
        // Chercher appsettings.json dans le projet API
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "TunNetCom.SilkRoadErp.Sales.Api");
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddUserSecrets<SalesContext>()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<SalesContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            // Fallback pour le design-time si pas de connection string
            connectionString = "Server=(localdb)\\mssqllocaldb;Database=SalesDb;Trusted_Connection=True;TrustServerCertificate=True;";
        }
        
        optionsBuilder.UseSqlServer(connectionString);

        return new SalesContext(optionsBuilder.Options);
    }
}

