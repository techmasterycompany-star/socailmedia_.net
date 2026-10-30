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

        public async Task<(IReadOnlyList<LiveStream> Items, int TotalCount)> GetAllAsync(int page, int pageSize)
        {
            var query = _context.LiveStreams.AsNoTracking();
            var totalCount = await query.CountAsync();
            var skip = ((long)page - 1) * pageSize;

            var items = skip >= totalCount
                ? []
                : await query.OrderByDescending(ls => ls.CreatedAt)
                    .ThenByDescending(ls => ls.Id)
                    .Skip((int)skip)
                    .Take(pageSize)
                    .ToListAsync();

            return (items, totalCount);
        }

        public async Task<bool> HasActiveStreamAsync(long hostId)
        {
            return await _context.LiveStreams.AnyAsync(ls => ls.HostId == hostId && ls.Status == LiveStreamStatus.Live);
        }

        public async Task<LiveStream?> GetByIdAsync(long streamId)
        {
            return await _context.LiveStreams.FirstOrDefaultAsync(ls => ls.Id == streamId);
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

        public async Task AddMessageAsync(StreamMessage message)
        {
            _context.StreamMessages.Add(message);
        }

        public async Task<(IReadOnlyList<StreamMessage> Items, int TotalCount)> GetMessagesByStreamIdAsync(
            long streamId,
            int page,
            int pageSize)
        {
            var query = _context.StreamMessages
                .Where(sm => sm.StreamId == streamId && sm.MessageType == StreamMessageType.Chat)
                .AsNoTracking();
            var totalCount = await query.CountAsync();
            var skip = ((long)page - 1) * pageSize;

            var items = skip >= totalCount
                ? []
                : await query.OrderByDescending(sm => sm.CreatedAt)
                    .ThenByDescending(sm => sm.Id)
                    .Skip((int)skip)
                    .Take(pageSize)
                    .OrderBy(sm => sm.CreatedAt)
                    .ThenBy(sm => sm.Id)
                    .ToListAsync();

            return (items, totalCount);
        }
    }
}