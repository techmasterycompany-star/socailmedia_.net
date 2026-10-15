using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Social_Media_Platform.Application.Interfaces;
using Social_Media_Platform.Application.Interfaces.IRepositories.Admin;
using Social_Media_Platform.Application.Interfaces.IRepositories.Moderator;
using Social_Media_Platform.Infrastructure.Data;
using Social_Media_Platform.Infrastructure.Options;
using Social_Media_Platform.Infrastructure.Repository;
using Social_Media_Platform.Infrastructure.Services;
using Social_Media_Platform.Infrastructure.Repositories.Admin;
using Social_Media_Platform.Infrastructure.Repositories.Moderator;

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


        services.AddScoped<IAdminUserRepository , AdminUserRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IModerationRepository, ModerationRepository>();
        services.AddScoped<IAdminCategoryRepository, AdminCategoryRepository>();
        services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();
        services.AddScoped<IAdminSettingsRepository, AdminSettingsRepository>();
        services.AddScoped<DbSeeder>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IOneTimeTokenRepository, OneTimeTokenRepository>();

        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordService, PasswordService>();

        services.Configure<EmailOptions>(configuration.GetSection("Email"));
        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));


        return services;
    }
}
