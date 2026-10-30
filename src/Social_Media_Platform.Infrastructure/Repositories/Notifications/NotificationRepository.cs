using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.DTOs.Notifications;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.Infrastructure.Repositories;

public sealed class NotificationRepository : INotificationRepository
{
    private readonly Social_Media_PlatformDbContext _context;

    public NotificationRepository(Social_Media_PlatformDbContext context)
    {
        _context = context;
    }
    public async Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetForRecipientAsync(long recipientId, int page, int pageSize)
    {
        var query = _context.Notifications
            .AsNoTracking()
            .Where(notification => notification.RecipientId == recipientId);
        var totalCount = await query.CountAsync();
        var skip = ((long)page - 1) * pageSize;

        var items = skip >= totalCount
            ? []
            : await query.OrderByDescending(notification => notification.CreatedAt)
                .ThenByDescending(notification => notification.Id)
                .Skip((int)skip)
                .Take(pageSize)
                .ToListAsync();

        return (items, totalCount);
    }

    public Task<int> GetUnreadCountAsync(long recipientId) =>
        _context.Notifications.CountAsync(
            notification => notification.RecipientId == recipientId && !notification.IsRead);

    public Task<Notification?> GetByIdForRecipientAsync(long notificationId, long recipientId) =>
        _context.Notifications.FirstOrDefaultAsync(
            notification => notification.Id == notificationId
                && notification.RecipientId == recipientId);

    public Task<bool> ExistsAsync(NotificationCreateRequest request) =>
        _context.Notifications.AnyAsync(
            notification => notification.RecipientId == request.recipientId
                && notification.ActorId == request.actorId
                && notification.Type == request.type
                && notification.FollowId == request.followId
                && notification.PostId == request.postId
                && notification.CommentId == request.commentId
                && notification.LiveStreamId == request.liveStreamId);

    public async Task AddAsync(Notification notification)
    {
        await _context.Notifications.AddAsync(notification);
    }

    public async Task AddRangeAsync(IReadOnlyCollection<Notification> notifications)
    {
        await _context.Notifications.AddRangeAsync(notifications);
    }

    public async Task<IReadOnlyList<long>> GetExistingLiveStreamNotificationRecipientIdsAsync(IReadOnlyCollection<long> recipientIds, long actorId, long liveStreamId)
    {
        if (recipientIds.Count == 0)
            return [];

        return await _context.Notifications
            .AsNoTracking()
            .Where(notification =>
                recipientIds.Contains(notification.RecipientId)
                && notification.ActorId == actorId
                && notification.LiveStreamId == liveStreamId
                && notification.Type == NotificationType.LiveStreamStarted)
            .Select(notification => notification.RecipientId)
            .Distinct()
            .ToListAsync();
    }

    public async Task SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public Task<int> MarkAllAsReadAsync(long recipientId) =>
        _context.Notifications
            .Where(notification => notification.RecipientId == recipientId && !notification.IsRead)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(notification => notification.IsRead, true)
                    .SetProperty(notification => notification.UpdatedAt, DateTime.UtcNow));
}
