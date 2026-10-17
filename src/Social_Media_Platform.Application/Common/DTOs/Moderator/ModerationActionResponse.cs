using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Common.DTOs.Moderator
{
    public sealed class ModerationActionResponse
    {
        public ModerationTargetType TargetType { get; init; }

        public long TargetId { get; init; }

        public ModerationAction Action { get; init; }

        public string Message { get; init; } = string.Empty;

        public DateTime ExecutedAt { get; init; }
    }
}
