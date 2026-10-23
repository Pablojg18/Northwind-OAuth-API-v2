using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace APIWeb.Data;

/// <summary>
/// Permite usar `dotnet ef` sin arrancar toda la aplicación.
/// </summary>
public class NorthwindContextFactory : IDesignTimeDbContextFactory<NorthwindContext>
{
    public NorthwindContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<NorthwindContext>()
            .UseMySQL(DesignTimeConfiguration.ConnectionString)
            .Options;

        return new NorthwindContext(options);
    }
}

public class AppIdentityDbContextFactory : IDesignTimeDbContextFactory<AppIdentityDbContext>
{
    public AppIdentityDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppIdentityDbContext>()
            .UseMySQL(DesignTimeConfiguration.ConnectionString)
            .Options;

        return new AppIdentityDbContext(options);
    }
}

internal static class DesignTimeConfiguration
{
    public static string ConnectionString { get; } = BuildConfiguration().GetConnectionString("ConexionMySQL")
        ?? "Server=localhost;Port=3306;Database=nwind;User Id=root;Password=root;";

    private static IConfiguration BuildConfiguration() =>
        new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
}
