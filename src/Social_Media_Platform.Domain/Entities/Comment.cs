namespace Social_Media_Platform.Domain.Entities;

public sealed class Comment : SoftDelete
{
    public long PostId { get; set; }

    public long AuthorId { get; set; }

    public string Content { get; set; } = string.Empty;

    public Post Post { get; set; } = null!;

    public User Author { get; set; } = null!;
}
