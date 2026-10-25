namespace Social_Media_Platform.Domain.Entities;

public sealed class Block : BaseEntity
{
    public long BlockerId { get; set; }

    public long BlockedUserId { get; set; }

    public User Blocker { get; set; } = null!;

    public User BlockedUser { get; set; } = null!;
}

