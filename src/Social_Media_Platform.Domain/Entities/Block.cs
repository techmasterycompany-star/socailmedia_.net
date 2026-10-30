namespace Social_Media_Platform.Domain.Entities;

public sealed class Block : BaseEntity
{
    public long BlockerId { get; set; }

    public long BlockedId { get; set; }

    public User Blocker { get; set; } = null!;

    public User Blocked { get; set; } = null!;
}
