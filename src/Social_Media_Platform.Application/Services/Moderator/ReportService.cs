using Social_Media_Platform.Application.Common.DTOs.Moderator;
using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Application.Interfaces.IRepositories.Moderator;
using Social_Media_Platform.Application.Interfaces.IServices.Moderator;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Services.Moderator
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository repo;
        public ReportService(IReportRepository repo) => this.repo = repo;



        private const int MaxReasonLength = 500;
        private const int MaxNotesLength = 2000;


        public async Task<List<ReportListItemDto>> GetReportsAsync() =>
            await repo.GetReportsAsync();

        public async Task<ReportDetailDto> AddModeratorNoteAsync(long id, long? actorId, AddModeratorNoteRequest request)
        {
            if(string.IsNullOrWhiteSpace(request.Note)) 
                throw new BadRequestException("A note is required");

            ValidateNotes(request.Note);
            
            var report = await repo.GetReportEntityAsync(id)
                   ?? throw new NotFoundException("The requested report could not be found");

            if (report.Status is ReportStatus.Actioned or ReportStatus.Dismissed)
                throw new ConflictException("Notes cannot be added to a closed report");

            report.DecisionNotes = AppendNote(report.DecisionNotes, request.Note);

            if (report.ReviewedById is null)
                report.ReviewedById = actorId;
            

            await repo.UpdateAsync(report);

            return await GetReportAsync(report.Id);
        }

        public async Task<ReportDetailDto> CreateReportAsync(long reporterId, CreateReportRequest request)
        {
            if (!Enum.IsDefined(request.TargetType))
                throw new BadRequestException("A valid report target type is required");

            if (request.TargetId <= 0)
                throw new BadRequestException("A valid target id is required");

            if (string.IsNullOrWhiteSpace(request.Reason))
                throw new BadRequestException("A report reason is required");
            

            var reason = request.Reason.Trim();

            if (reason.Length > MaxReasonLength)
                throw new BadRequestException($"The report reason cannot exceed {MaxReasonLength} characters");
            

            var targetExists = await repo.TargetExistsAsync(request.TargetType, request.TargetId);
            if (!targetExists)
                throw new NotFoundException("The reported content could not be found");
            

            var isDuplicate = await repo.HasDuplicateAsync(reporterId,request.TargetType,request.TargetId);
            if (isDuplicate)
                throw new ConflictException("You have already reported this content");
            

            var report = new Report
            {
                ReporterId = reporterId,
                TargetType = request.TargetType,
                TargetId = request.TargetId,
                Reason = reason,
                Status = ReportStatus.Pending
            };

            await repo.AddAsync(report);
         
            return await GetReportAsync(report.Id);
        }

        public async Task<ReportDetailDto> DismissReportAsync(long id,long? actorId, string? reason)
        {
            ValidateNotes(reason);

            var report = await GetOpenReportAsync(id);

            if (report.Status is not (ReportStatus.Pending or ReportStatus.Reviewed))
                throw new ConflictException("Only pending or reviewed reports can be dismissed.");
            

            report.Status = ReportStatus.Dismissed;
            report.ReviewedById = actorId;
            report.DecisionNotes = AppendNote(report.DecisionNotes, reason);

            await repo.UpdateAsync(report);

            return await GetReportAsync(report.Id);
        }

        public async Task<ReportDetailDto> GetReportAsync(long id)
        {
            var report = await repo.GetReportAsync(id)
                ?? throw new NotFoundException("The requested report could not be found");

            return report;
        }

        public async Task<ReportDetailDto> ResolveReportAsync(long id,long? actorId, ResolveReportRequest request)
        {                     
            ValidateNotes(request.Notes);

            var report = await GetOpenReportAsync(id);

            if (report.Status is not (ReportStatus.Pending or ReportStatus.Reviewed))
                throw new ConflictException("Only pending or reviewed reports can be resolved.");
            

            report.Status = ReportStatus.Actioned;
            report.ReviewedById = actorId;
            report.Resolution = request.Action;
            report.DecisionNotes = AppendNote(report.DecisionNotes, request.Notes);

            await repo.UpdateAsync(report);
           
            return await GetReportAsync(report.Id);
        }

        public async Task<ReportDetailDto> StartReviewAsync(long id,long? actorId, string? notes)
        {
            ValidateNotes(notes);

            var report = await GetOpenReportAsync(id);

            if (report.Status != ReportStatus.Pending)
            {
                throw new ConflictException("Only pending reports can be moved into review.");
            }

            report.Status = ReportStatus.Reviewed;
            report.ReviewedById = actorId;
            report.DecisionNotes = AppendNote(report.DecisionNotes, notes);

            await repo.UpdateAsync(report);
            return await GetReportAsync(report.Id);
        }





        private static void ValidateNotes(string? notes)
        {
            if (notes is { Length: > MaxNotesLength })
            {
                throw new BadRequestException($"Notes cannot exceed {MaxNotesLength} characters");
            }
        }
        private static string? AppendNote(string? existing, string? note)
        {
            if (string.IsNullOrWhiteSpace(note))
            {
                return existing;
            }

            var stamped = $"[{DateTime.UtcNow:O}] {note.Trim()}";

            return string.IsNullOrWhiteSpace(existing) ? stamped : $"{existing}{Environment.NewLine}{stamped}";
        }

        private async Task<Report> GetOpenReportAsync(long id) =>
           await repo.GetReportEntityAsync(id)
                ?? throw new NotFoundException("The requested report could not be found.");
    }
}
