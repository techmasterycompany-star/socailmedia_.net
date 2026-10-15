using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Social_Media_Platform.Application.Interfaces;
using Social_Media_Platform.Application.Options;
using Social_Media_Platform.Application.Services;

namespace Social_Media_Platform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.Configure<AuthSettings>(configuration.GetSection("Auth"));

        return services;
    }
}
