using Microsoft.Extensions.DependencyInjection;
using Social_Media_Platform.Application.Interfaces.IServices;
using Social_Media_Platform.Application.Services.LiveStreaming;

namespace Social_Media_Platform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILiveStreamService, LiveStreamService>();
        return services;
    }

}
