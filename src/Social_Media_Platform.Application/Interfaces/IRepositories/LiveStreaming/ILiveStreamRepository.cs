using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Application.Interfaces.IRepositories
{
    public interface ILiveStreamRepository
    {
        Task<IEnumerable<LiveStream>> GetAllAsync();
        Task<bool> HasActiveStreamAsync(long hostId);
        Task<LiveStream?> GetByIdAsync(long hostId);
        Task<LiveStream?> GetByRoomNameAsync(string roomName);
        Task AddLiveStreamAsync(LiveStream liveStream);
        Task SaveChangesAsync();
    }
}