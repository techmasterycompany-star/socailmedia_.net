using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.Common.DTOs.Moderator;
using Social_Media_Platform.Application.Interfaces.IRepositories.Moderator;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.Infrastructure.Repositories.Moderator
{
    public class ReportRepository : IReportRepository
    {
        private readonly Social_Media_PlatformDbContext context;
        public ReportRepository(Social_Media_PlatformDbContext context) => this.context = context;




        public async Task AddAsync(Report report)
        {
            await context.AddAsync(report);
            await context.SaveChangesAsync();
        }

        public Task<Report?> GetReportEntityAsync(long id) =>
           context.Reports
            .Include(report => report.Reporter)
            .Include(report => report.Reviewer)
            .FirstOrDefaultAsync(report => report.Id == id);


        public Task<ReportDetailDto?> GetReportAsync(long id) =>
              context.Reports
                 .AsNoTracking()
                 .Where(report => report.Id == id)
                 .Select(report => new ReportDetailDto
                 {
                     Id = report.Id,
                     ReporterId = report.ReporterId,
                     ReporterUsername = report.Reporter.Username,
                     TargetType = report.TargetType,
                     TargetId = report.TargetId,
                     Reason = report.Reason,
                     Status = report.Status ?? ReportStatus.Pending,
                     ReviewedById = report.ReviewedById,
                     ReviewerUsername = report.Reviewer != null ? report.Reviewer.Username : null,
                     DecisionNotes = report.DecisionNotes,
                     Resolution = report.Resolution,
                     Details =
                         report.TargetType == ReportTargetType.Post
                             ? context.Posts.Where(post => post.Id == report.TargetId).Select(post => post.Content).FirstOrDefault()
                             : report.TargetType == ReportTargetType.Comment
                                 ? context.Comments.Where(comment => comment.Id == report.TargetId).Select(comment => comment.Content).FirstOrDefault()
                                 : report.TargetType == ReportTargetType.Profile
                                     ? context.Profiles.Where(profile => profile.Id == report.TargetId).Select(profile => profile.Bio).FirstOrDefault()
                                     : report.TargetType == ReportTargetType.Stream
                                         ? context.LiveStreams.Where(stream => stream.Id == report.TargetId).Select(stream => stream.Title).FirstOrDefault()
                                         : null,
                     CreatedAt = report.CreatedAt,
                     UpdatedAt = report.UpdatedAt
                 })
            .FirstOrDefaultAsync();


        public async Task<List<ReportListItemDto>> GetReportsAsync() =>
            await context.Reports
            .AsNoTracking()
            .Select(report => new ReportListItemDto
            {
                Id = report.Id,
                ReporterId = report.ReporterId,
                ReporterUsername = report.Reporter.Username,
                TargetType = report.TargetType,
                TargetId = report.TargetId,
                Reason = report.Reason,
                Status = report.Status ?? ReportStatus.Pending,
                ReviewedById = report.ReviewedById,
                ReviewerUsername = report.Reviewer != null ? report.Reviewer.Username : null,
                CreatedAt = report.CreatedAt,
                UpdatedAt = report.UpdatedAt
            })
            .ToListAsync();

        public Task<bool> HasDuplicateAsync(long reporterId, ReportTargetType targetType, long targetId) =>
            context.Reports.AsNoTracking()
                .AnyAsync(report => report.ReporterId == reporterId && report.TargetType == targetType && report.TargetId == targetId && report.Status != ReportStatus.Dismissed);


        public Task<bool> TargetExistsAsync(ReportTargetType targetType, long targetId) =>
            targetType switch
            {
                ReportTargetType.Post => context.Posts.AnyAsync(post => post.Id == targetId),
                ReportTargetType.Comment => context.Comments.AnyAsync(comment => comment.Id == targetId),
                ReportTargetType.Profile => context.Profiles.AnyAsync(profile => profile.Id == targetId),
                ReportTargetType.Stream => context.LiveStreams.AnyAsync(stream => stream.Id == targetId),
                _ => Task.FromResult(false)
            };


        public async Task UpdateAsync(Report report)
        {
            context.Update(report);
            await context.SaveChangesAsync();
        }
    }
}
