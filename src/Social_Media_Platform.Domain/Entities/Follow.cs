using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Domain.Entities;

public sealed class Follow : BaseEntity
{
    public long FollowerId { get; set; }

    public long FollowingId { get; set; }

    public FollowStatus Status { get; set; }

    public User Follower { get; set; } = null!;

    public User Following { get; set; } = null!;
}
