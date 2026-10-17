using Social_Media_Platform.Application.DTOS.LiveStream;

namespace Social_Media_Platform.Application.Interfaces.IServices
{
    public interface ILiveStreamService
    {
        Task<IEnumerable<LiveStreamDto>> GetAllAsync();
        Task<StartLiveStreamResponse> ExecuteStartAsync(long hostId, StartLiveStreamRequest request);
        Task<StartLiveStreamResponse> ExecuteJoinAsync(long viewerId, long streamId);
        Task ExecuteEndAsync(long hostId, long streamId);
    }
}