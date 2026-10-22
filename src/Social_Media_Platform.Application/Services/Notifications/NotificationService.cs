using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOs.Notifications;
using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Application.Interfaces.IServices;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Services;

public sealed class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IFollowRepository _followRepository;
    private readonly INotificationPublisher _notificationPublisher;
    private const int MaximumPageSize = 50;

    public NotificationService(INotificationRepository notificationRepository, IFollowRepository followRepository, INotificationPublisher notificationPublisher)
    {
        _notificationRepository = notificationRepository;
        _followRepository = followRepository;
        _notificationPublisher = notificationPublisher;
    }

    public async Task<PagedResult<NotificationResponse>> GetForRecipientAsync(long recipientId, int page, int pageSize)
    {
        ValidateUserId(recipientId);
        ValidatePagination(page, pageSize);

        var (items, totalCount) = await _notificationRepository.GetForRecipientAsync(
            recipientId,
            page,
            pageSize);

        return PagedResult<NotificationResponse>.Create(
            items.Select(Map).ToList(),
            page,
            pageSize,
            totalCount);
    }

    public Task<int> GetUnreadCountAsync(long recipientId)
    {
        ValidateUserId(recipientId);
        return _notificationRepository.GetUnreadCountAsync(recipientId);
    }

    public async Task<NotificationResponse> MarkAsReadAsync(long recipientId, long notificationId)
    {
        ValidateUserId(recipientId);
        if (notificationId <= 0)
            throw new ArgumentOutOfRangeException(nameof(notificationId));

        var notification = await _notificationRepository.GetByIdForRecipientAsync(
            notificationId,
            recipientId);

        if (notification is null)
            throw new NotFoundException($"Notification with ID {notificationId} was not found.");

        if (!notification.IsRead)
        {
            notification.IsRead = true;
            await _notificationRepository.SaveChangesAsync();
        }

        return Map(notification);
    }

    public Task<int> MarkAllAsReadAsync(long recipientId)
    {
        ValidateUserId(recipientId);
        return _notificationRepository.MarkAllAsReadAsync(recipientId);
    }

    public async Task<NotificationResponse?> CreateAsync(NotificationCreateRequest request)
    {
        ValidateUserId(request.recipientId);
        ValidateUserId(request.actorId);

        if (!Enum.IsDefined(request.type))
            throw new ArgumentOutOfRangeException("Notification Type Is Required");

        if (request.recipientId == request.actorId)
            return null;

        var exists = await _notificationRepository.ExistsAsync(request);
        if (exists)
            return null;

        var notification = new Notification
        {
            RecipientId = request.recipientId,
            ActorId = request.actorId,
            Type = request.type,
            FollowId = request.followId,
            PostId = request.postId,
            CommentId = request.commentId,
            LiveStreamId = request.liveStreamId
        };

        await _notificationRepository.AddAsync(notification);
        await _notificationRepository.SaveChangesAsync();

        var response = Map(notification);
        await _notificationPublisher.PublishAsync(request.recipientId, response);
        return response;
    }

    public async Task NotifyLiveStreamStartedAsync(long hostId, long liveStreamId)
    {
        ValidateUserId(hostId);

        if (liveStreamId <= 0)
            throw new ArgumentOutOfRangeException(nameof(liveStreamId));

        var followerIds = await _followRepository.GetAcceptedFollowerIdsAsync(hostId);

        var recipients = followerIds.Where(id => id != hostId).Distinct().ToList();

        if (recipients.Count == 0)
            return;

        var existingRecipientIds =
            await _notificationRepository
                .GetExistingLiveStreamNotificationRecipientIdsAsync(
                    recipients,
                    hostId,
                    liveStreamId);

        var existingIds = existingRecipientIds.ToHashSet();

        var notifications = recipients
            .Where(id => !existingIds.Contains(id))
            .Select(recipientId => new Notification
            {
                RecipientId = recipientId,
                ActorId = hostId,
                Type = NotificationType.LiveStreamStarted,
                LiveStreamId = liveStreamId
            })
            .ToList();

        if (notifications.Count == 0)
            return;

        await _notificationRepository.AddRangeAsync(notifications);
        await _notificationRepository.SaveChangesAsync();

        foreach (var notification in notifications)
        {
            await _notificationPublisher.PublishAsync(
                notification.RecipientId,
                Map(notification));
        }
    }

    private static NotificationResponse Map(Notification notification) =>
        new(
            notification.Id,
            notification.ActorId,
            notification.Type,
            notification.IsRead,
            notification.FollowId,
            notification.PostId,
            notification.CommentId,
            notification.LiveStreamId,
            notification.CreatedAt);

    private static void ValidateUserId(long userId)
    {
        if (userId <= 0)
            throw new ArgumentOutOfRangeException("User ID must be a positive number.");
    }

    private static void ValidatePagination(int page, int pageSize)
    {
        if (page <= 0)
            throw new ArgumentOutOfRangeException("Page must be a positive number.");
        if (pageSize <= 0 || pageSize > MaximumPageSize)
            throw new ArgumentOutOfRangeException($"Page size must be between 1 and {MaximumPageSize}.");
    }
}
