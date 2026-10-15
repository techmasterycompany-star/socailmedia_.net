using Social_Media_Platform.Application.Common.DTOs.Moderator;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Interfaces.IRepositories.Moderator
{
    public interface IReportRepository
    {
        Task<List<ReportListItemDto>> GetReportsAsync();
        Task<ReportDetailDto?> GetReportAsync(long id);
        Task<Report?> GetReportEntityAsync(long id);   
        Task<bool> HasDuplicateAsync(long reporterId, ReportTargetType targetType, long targetId);
        Task AddAsync(Report report);
        Task UpdateAsync(Report report);
        Task<bool> TargetExistsAsync(ReportTargetType targetType, long targetId);
    }
}
