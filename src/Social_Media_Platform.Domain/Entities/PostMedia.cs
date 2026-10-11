using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Domain.Entities;

public sealed class PostMedia : BaseEntity
{
    public long PostId { get; set; }

    public MediaType MediaType { get; set; }

    public string MediaUrl { get; set; } = string.Empty;

    public Post Post { get; set; } = null!;
}
