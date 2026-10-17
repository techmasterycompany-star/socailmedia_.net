using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.DTOS.LiveStream;

public class LiveStreamDto
{
    public long Id { get; set; }
    public long HostId { get; set; }
    public string Title { get; set; } = string.Empty;
    public LiveStreamStatus Status { get; set; }
    public AudienceSetting AudienceSetting { get; set; }
    public string CreatedAt { get; set; } = string.Empty;
}