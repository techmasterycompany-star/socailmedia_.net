using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOs.Notifications;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Interfaces.IServices;

public interface INotificationService
{
    Task<PagedResult<NotificationResponse>> GetForRecipientAsync(long recipientId, int page, int pageSize);

    Task<int> GetUnreadCountAsync(long recipientId);

    Task<NotificationResponse> MarkAsReadAsync(long recipientId, long notificationId);

    Task<int> MarkAllAsReadAsync(long recipientId);

    Task<NotificationResponse?> CreateAsync(NotificationCreateRequest request);

    Task NotifyLiveStreamStartedAsync(
        long hostId,
        long liveStreamId);
}
