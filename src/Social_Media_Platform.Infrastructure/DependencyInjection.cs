using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<Social_Media_PlatformDbContext>(
            options =>
            {
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException(
                        "The 'DefaultConnection' connection string is missing. Configure it with " +
                        "ConnectionStrings__DefaultConnection.");
                }

                options.UseSqlServer(connectionString);
            });
        services.AddScoped<DbSeeder>();

        return services;
    }
}
