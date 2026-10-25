namespace Social_Media_Platform.Application.DTOS.LiveStreaming;

public sealed class StreamChatMessageDTO
{
    public long Id { get; init; }

    public long StreamId { get; init; }

    public long SenderId { get; init; }

    public string Content { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }
}
