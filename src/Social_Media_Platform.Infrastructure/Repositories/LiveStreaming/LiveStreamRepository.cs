using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Infrastructure.Data;
using Social_Media_Platform.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Infrastructure.Repositories.LiveStreaming
{
    public class LiveStreamRepository : ILiveStreamRepository
    {
        private readonly Social_Media_PlatformDbContext _context;

        public LiveStreamRepository(Social_Media_PlatformDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LiveStream>> GetAllAsync()
        {
            return await _context.LiveStreams.ToListAsync();
        }
        public async Task<bool> HasActiveStreamAsync(long hostId)
        {
            return await _context.LiveStreams.AnyAsync(ls => ls.HostId == hostId && ls.Status == LiveStreamStatus.Live);
        }

        public async Task<LiveStream?> GetByIdAsync(long hostId)
        {
            return await _context.LiveStreams.FirstOrDefaultAsync(ls => ls.HostId == hostId);
        }

        public async Task<LiveStream?> GetByRoomNameAsync(string roomName)
        {
            return await _context.LiveStreams.FirstOrDefaultAsync(ls => ls.RoomName == roomName);
        }

        public async Task AddLiveStreamAsync(LiveStream liveStream)
        {
            await _context.LiveStreams.AddAsync(liveStream);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}