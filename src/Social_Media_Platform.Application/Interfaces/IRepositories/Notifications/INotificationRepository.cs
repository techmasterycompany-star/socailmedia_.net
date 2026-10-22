using Social_Media_Platform.Application.DTOs.Notifications;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Interfaces.IRepositories;

public interface INotificationRepository
{
    Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetForRecipientAsync(
        long recipientId,
        int page,
        int pageSize);

    Task<int> GetUnreadCountAsync(long recipientId);

    Task<Notification?> GetByIdForRecipientAsync(
        long notificationId,
        long recipientId);

    Task<bool> ExistsAsync(NotificationCreateRequest request);

    Task AddAsync(Notification notification);

    Task AddRangeAsync(
        IReadOnlyCollection<Notification> notifications);

    Task<IReadOnlyList<long>> GetExistingLiveStreamNotificationRecipientIdsAsync(
        IReadOnlyCollection<long> recipientIds,
        long actorId,
        long liveStreamId);

    Task SaveChangesAsync();

    Task<int> MarkAllAsReadAsync(long recipientId);
}
