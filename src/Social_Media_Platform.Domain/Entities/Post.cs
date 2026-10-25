using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Domain.Entities;
public sealed class Post : SoftDelete
{
    public long AuthorId { get; set; }
    public string? Content { get; set; }
    public PostVisibility Visibility { get; set; } = PostVisibility.Public;
    public long? CategoryId { get; set; }
    public User Author { get; set; } = null!;
    public Category? Category { get; set; }
    public bool IsHidden { get; set; } = false;
    public ICollection<PostMedia> Media { get; set; } = new List<PostMedia>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
    public ICollection<PostHashtag> PostHashtags { get; set; } = new List<PostHashtag>();
    public ICollection<SavedPost> SavedBy { get; set; } = new List<SavedPost>();
}

public sealed class SavedPost : BaseEntity
{
    public long UserId { get; set; }
    public long PostId { get; set; }
    public User User { get; set; } = null!;
    public Post Post { get; set; } = null!;
}

public sealed class Hashtag : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public ICollection<PostHashtag> PostHashtags { get; set; } = new List<PostHashtag>();
}

public sealed class PostHashtag : BaseEntity
{
    public long PostId { get; set; }
    public long HashtagId { get; set; }
    public Post Post { get; set; } = null!;
    public Hashtag Hashtag { get; set; } = null!;
}