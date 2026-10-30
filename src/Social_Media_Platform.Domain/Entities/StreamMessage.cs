using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Domain.Entities;

public sealed class StreamMessage : BaseEntity
{
    public long StreamId { get; set; }

    public long SenderId { get; set; }

    public StreamMessageType? MessageType { get; set; }

    public string? Content { get; set; }

    public LiveStream Stream { get; set; } = null!;

    public User Sender { get; set; } = null!;
}
