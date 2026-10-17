namespace Social_Media_Platform.Application.DTOS.LiveStream
{
    public class StartLiveStreamResponse
    {
        public long StreamId { get; set; }
        public string RoomName { get; set; } = null!;
        public string ServerUrl { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}