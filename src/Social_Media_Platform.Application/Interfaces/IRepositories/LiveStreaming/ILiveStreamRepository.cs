using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Application.Interfaces.IRepositories
{
    public interface ILiveStreamRepository
    {
        Task<(IReadOnlyList<LiveStream> Items, int TotalCount)> GetAllAsync(int page, int pageSize);
        Task<bool> HasActiveStreamAsync(long hostId);
        Task<LiveStream?> GetByIdAsync(long streamId);
        Task<LiveStream?> GetByRoomNameAsync(string roomName);
        Task AddLiveStreamAsync(LiveStream liveStream);
        Task AddMessageAsync(StreamMessage message);
        Task<(IReadOnlyList<StreamMessage> Items, int TotalCount)> GetMessagesByStreamIdAsync(
            long streamId,
            int page,
            int pageSize);
        Task SaveChangesAsync();
    }
}