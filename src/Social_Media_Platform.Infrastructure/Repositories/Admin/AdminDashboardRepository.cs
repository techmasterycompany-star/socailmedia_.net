using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.Interfaces.IRepositories.Admin;
using Social_Media_Platform.Domain.Enums;
using Social_Media_Platform.Infrastructure.Data;
using System.Threading;

namespace Social_Media_Platform.Infrastructure.Repositories.Admin
{
    public class AdminDashboardRepository : IAdminDashboardRepository
    {
        private readonly Social_Media_PlatformDbContext context;
        public AdminDashboardRepository(Social_Media_PlatformDbContext context) => this.context = context;




        public async Task<int> CountCommentsAsync() =>
            await context.Comments.AsNoTracking().CountAsync();


        public async Task<int> CountFollowsAsync() =>
            await context.Follows.AsNoTracking().CountAsync();



        public async Task<int> CountLikesAsync() =>
            await context.PostLikes.AsNoTracking().CountAsync();



        public async Task<int> CountPostsAsync() =>
            await context.Posts.AsNoTracking().CountAsync();



        public async Task<Dictionary<ReportStatus, int>> CountReportsByStatusAsync()
        {
            var counts = await context.Reports.AsNoTracking()
                .GroupBy(report => report.Status ?? ReportStatus.Pending)
                .Select(group => new { Status = group.Key, Count = group.Count() })
                .ToListAsync();

            return counts.ToDictionary(item => item.Status, item => item.Count);
        }

        public async Task<Dictionary<LiveStreamStatus, int>> CountStreamsByStatusAsync()
        {
            var counts = await context.LiveStreams.AsNoTracking()
                .GroupBy(stream => stream.Status)
                .Select(group => new { Status = group.Key, Count = group.Count() })
                .ToListAsync();

            return counts.ToDictionary(item => item.Status, item => item.Count);
        }

        public Task<int> CountUsersAsync(UserStatus? status)
        {
            var users = context.Users.AsNoTracking();
            if (status.HasValue)
            {
                users = status.Value == UserStatus.Active
                ? users.Where(user => !user.Status.HasValue || user.Status == UserStatus.Active)
                : users.Where(user => user.Status == status.Value);
            }
            return users.CountAsync();
        }

        public async Task<int> CountVerifiedUsersAsync() =>
             await context.Users.AsNoTracking().CountAsync(user => user.IsVerified);


    }
}
