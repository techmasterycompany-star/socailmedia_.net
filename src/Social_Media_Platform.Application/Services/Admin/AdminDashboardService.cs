using Social_Media_Platform.Application.Common.DTOs.Admin;
using Social_Media_Platform.Application.Interfaces.IRepositories.Admin;
using Social_Media_Platform.Application.Interfaces.IServices.Admin;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Services.Admin
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IAdminDashboardRepository repo;
        public AdminDashboardService(IAdminDashboardRepository repo) => this.repo = repo;
   
        


        public async Task<AdminDashboardStatsDto> GetStatsAsync()
        {
            var reportCounts = await repo.CountReportsByStatusAsync();
            var streamCounts = await repo.CountStreamsByStatusAsync();

            return new AdminDashboardStatsDto
            {
                Users = new UserStatisticsDto
                {
                    Total = await repo.CountUsersAsync(null),
                    Active = await repo.CountUsersAsync(UserStatus.Active),
                    Suspended = await repo.CountUsersAsync(UserStatus.Suspended),
                    Blocked = await repo.CountUsersAsync(UserStatus.Blocked),
                    Verified = await repo.CountVerifiedUsersAsync(),
                },
                Content = new ContentStatisticsDto
                {
                    TotalPosts = await repo.CountPostsAsync(),
                    TotalComments = await repo.CountCommentsAsync(),
                    TotalLikes = await repo.CountLikesAsync(),
                    TotalFollows = await repo.CountFollowsAsync()
                },
                Reports = new ReportStatisticsDto
                {
                    Total = reportCounts.Values.Sum(),
                    Pending = reportCounts.GetValueOrDefault(ReportStatus.Pending),
                    Reviewed = reportCounts.GetValueOrDefault(ReportStatus.Reviewed),
                    Actioned = reportCounts.GetValueOrDefault(ReportStatus.Actioned),
                    Dismissed = reportCounts.GetValueOrDefault(ReportStatus.Dismissed),
                },
                Streams = new StreamStatisticsDto
                {
                    Total = streamCounts.Values.Sum(),
                    Scheduled = streamCounts.GetValueOrDefault(LiveStreamStatus.Scheduled),
                    Live = streamCounts.GetValueOrDefault(LiveStreamStatus.Live),
                    Ended = streamCounts.GetValueOrDefault(LiveStreamStatus.Ended),
                    Cancelled = streamCounts.GetValueOrDefault(LiveStreamStatus.Cancelled)
                },
                GeneratedAt = DateTime.UtcNow
            };
        }
    }
}
