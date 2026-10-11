using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Domain.Entities;

public sealed class Report : BaseEntity
{
    public long ReporterId { get; set; }

    public ReportTargetType TargetType { get; set; }

    public long TargetId { get; set; }

    public string Reason { get; set; } = string.Empty;

    public ReportStatus? Status { get; set; }

    public long? ReviewedById { get; set; }

    public string? DecisionNotes { get; set; }

    public User Reporter { get; set; } = null!;

    public User? Reviewer { get; set; }
}
