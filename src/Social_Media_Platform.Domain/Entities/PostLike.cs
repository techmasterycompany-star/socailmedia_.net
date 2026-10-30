namespace Social_Media_Platform.Domain.Entities;

public sealed class PostLike : BaseEntity
{
    public long PostId { get; set; }

    public long UserId { get; set; }

    public Post Post { get; set; } = null!;

    public User User { get; set; } = null!;
}
