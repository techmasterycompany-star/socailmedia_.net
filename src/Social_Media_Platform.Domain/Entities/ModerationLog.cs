using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Domain.Entities;

public sealed class ModerationLog : BaseEntity
{
    public long ActorId { get; set; }

    public ModerationTargetType TargetType { get; set; }

    public long TargetId { get; set; }

    public ModerationAction Action { get; set; }

    public string? Reason { get; set; }

    public User Actor { get; set; } = null!;
}
