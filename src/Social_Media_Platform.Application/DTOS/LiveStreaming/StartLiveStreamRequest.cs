using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.DTOS.LiveStream
{
    public class StartLiveStreamRequest
    {
        public string Title { get; set; } = null!;
        public AudienceSetting AudienceSetting { get; set; }
    }
}