using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Domain.Entities;

public sealed class PostMedia : BaseEntity
{
    public long PostId { get; set; }
    public string PublicId { get; set; } = string.Empty;
    public MediaType MediaType { get; set; }
    public string DeliveryType { get; set; } = "upload";
    public string MediaUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public int DisplayOrder { get; set; }

    public Post Post { get; set; } = null!;
}
