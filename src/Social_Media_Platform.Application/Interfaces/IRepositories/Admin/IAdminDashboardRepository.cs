using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Interfaces.IRepositories.Admin
{
    public interface IAdminDashboardRepository
    {
        Task<int> CountUsersAsync(UserStatus? status);
        Task<int> CountVerifiedUsersAsync();
        Task<int> CountPostsAsync();
        Task<int> CountCommentsAsync();
        Task<int> CountLikesAsync();
        Task<int> CountFollowsAsync();

        Task<Dictionary<ReportStatus, int>> CountReportsByStatusAsync();
        Task<Dictionary<LiveStreamStatus, int>> CountStreamsByStatusAsync();
    }
}
