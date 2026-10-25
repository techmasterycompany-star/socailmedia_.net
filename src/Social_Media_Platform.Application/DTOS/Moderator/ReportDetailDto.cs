using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.DTOS.Moderator
{
    public class ReportDetailDto
    {
        public long Id { get; init; }

        public long ReporterId { get; init; }

        public string ReporterUsername { get; init; } = string.Empty;

        public ReportTargetType TargetType { get; init; }

        public long TargetId { get; init; }

        public string Reason { get; init; } = string.Empty;

        public ReportStatus Status { get; init; }

        public long? ReviewedById { get; init; }

        public string? ReviewerUsername { get; init; }

        public string? DecisionNotes { get; init; }

        public ReportResolution? Resolution { get; init; }

        public string? Details { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; init; }

    }
}
