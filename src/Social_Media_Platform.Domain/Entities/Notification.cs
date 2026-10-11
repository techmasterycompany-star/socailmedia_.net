using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Domain.Entities;

public sealed class Notification : BaseEntity
{
    public long RecipientId { get; set; }

    public long ActorId { get; set; }

    public NotificationType Type { get; set; }

    public bool IsRead { get; set; }

    public User Recipient { get; set; } = null!;

    public User Actor { get; set; } = null!;
}
