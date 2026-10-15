using Social_Media_Platform.Application.Common.DTOs.Moderator;

namespace Social_Media_Platform.Application.Interfaces.IServices.Moderator
{
    public interface IReportService
    {
        Task<ReportDetailDto> CreateReportAsync(long reporterId, CreateReportRequest request);

        Task<List<ReportListItemDto>> GetReportsAsync();

        Task<ReportDetailDto> GetReportAsync(long id);

        Task<ReportDetailDto> StartReviewAsync(long id, long? actorId, string? notes);

        Task<ReportDetailDto> ResolveReportAsync(long id,long? actorId, ResolveReportRequest request);

        Task<ReportDetailDto> DismissReportAsync(long id, long? actorId, string? reason);

        Task<ReportDetailDto> AddModeratorNoteAsync(long id, long? actorId, AddModeratorNoteRequest request);
    }
}
