using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.API;

public sealed class Social_Media_PlatformDbContextFactory
    : IDesignTimeDbContextFactory<Social_Media_PlatformDbContext>
{
    public Social_Media_PlatformDbContext CreateDbContext(string[] args)
    {
        DotEnvLoader.Load();
        var apiSettingsDirectory = FindApiSettingsDirectory();
        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiSettingsDirectory ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile(
                $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
                optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "The 'DefaultConnection' connection string is required for EF Core tooling. " +
                "Configure ConnectionStrings__DefaultConnection before running EF commands.");
        }

        var options = new DbContextOptionsBuilder<Social_Media_PlatformDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new Social_Media_PlatformDbContext(options);
    }

    private static string? FindApiSettingsDirectory()
    {
        for (var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
             directory is not null;
             directory = directory.Parent)
        {
            if (File.Exists(Path.Combine(directory.FullName, "appsettings.json")))
            {
                return directory.FullName;
            }

            var apiDirectories = directory.GetDirectories("*.API")
                .Concat(Directory.Exists(Path.Combine(directory.FullName, "src"))
                    ? new DirectoryInfo(Path.Combine(directory.FullName, "src")).GetDirectories("*.API")
                    : []);
            var apiDirectory = apiDirectories
                .FirstOrDefault(candidate => File.Exists(Path.Combine(candidate.FullName, "appsettings.json")));
            if (apiDirectory is not null)
            {
                return apiDirectory.FullName;
            }
        }

        return null;
    }
}
