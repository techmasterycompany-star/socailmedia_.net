using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Domain.Entities;

public sealed class LiveStream : BaseEntity
{
    public long HostId { get; set; }

    public string Title { get; set; } = string.Empty;

    public AudienceSetting? AudienceSetting { get; set; }

    public LiveStreamStatus? Status { get; set; }

    public string? RecordingUrl { get; set; }

    public User Host { get; set; } = null!;

    public ICollection<StreamMessage> Messages { get; set; } = new List<StreamMessage>();
}
