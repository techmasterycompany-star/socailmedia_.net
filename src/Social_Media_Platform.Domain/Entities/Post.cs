namespace Social_Media_Platform.Domain.Entities;

public sealed class Post : BaseEntity
{
    public long AuthorId { get; set; }

    public string? Content { get; set; }

    public bool IsPublic { get; set; } = true;

    public User Author { get; set; } = null!;

    public ICollection<PostMedia> Media { get; set; } = new List<PostMedia>();

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
}
