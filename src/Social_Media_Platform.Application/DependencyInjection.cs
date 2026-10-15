using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Social_Media_Platform.Application.Interfaces;
using Social_Media_Platform.Application.Options;
using Social_Media_Platform.Application.Services;
using Social_Media_Platform.Application.Interfaces.IServices.Admin;
using Social_Media_Platform.Application.Interfaces.IServices.Moderator;
using Social_Media_Platform.Application.Services.Admin;
using Social_Media_Platform.Application.Services.Moderator;

namespace Social_Media_Platform.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<IModerationService, ModerationService>();
        services.AddScoped<IAdminCategoryService, AdminCategoryService>();
        services.AddScoped<IAdminDashboardService, AdminDashboardService>();
        services.AddScoped<IAdminSettingsService, AdminSettingsService>();
        services.AddScoped<IAuthService, AuthService>();
        services.Configure<AuthSettings>(configuration.GetSection("Auth"));

        return services;
    }
}
