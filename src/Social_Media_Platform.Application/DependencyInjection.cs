using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Application.Interfaces.IServices;
using Social_Media_Platform.Application.Interfaces.IServices.Admin;
using Social_Media_Platform.Application.Interfaces.IServices.Moderator;
using Social_Media_Platform.Application.Options;
using Social_Media_Platform.Application.Services;
using Social_Media_Platform.Application.Services.Admin;
using Social_Media_Platform.Application.Services.LiveStreaming;
using Social_Media_Platform.Application.Services.Moderator;
using Social_Media_Platform.Application.Services.Posts;
using Social_Media_Platform.Application.Interfaces.IServices.LiveStreaming;

namespace Social_Media_Platform.Application;

public static class DependencyInjection
{
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
                services.AddScoped<ILiveStreamService, LiveStreamService>();
                services.AddScoped<IAdminUserService, AdminUserService>();
                services.AddScoped<IReportService, ReportService>();
                services.AddScoped<IModerationService, ModerationService>();
                services.AddScoped<IAdminCategoryService, AdminCategoryService>();
                services.AddScoped<IAdminDashboardService, AdminDashboardService>();
                services.AddScoped<IAdminSettingsService, AdminSettingsService>();
                services.AddScoped<IAuthService, AuthService>();
                services.AddScoped<ILiveStreamAccessService, LiveStreamAccessService>();
                services.AddScoped<INotificationService, NotificationService>();
                services.Configure<AuthSettings>(configuration.GetSection("Auth"));

            services.Configure<PostsMediaOptions>(configuration.GetSection("PostsMedia"));
            services.AddScoped<IUploadFileValidator, UploadFileValidator>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IPostInteractionService, PostInteractionService>();
            services.AddScoped<ISearchService, SearchService>();
            return services;
        }

}
