using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.DTOs.Notifications;

public sealed record NotificationResponse(
    long Id,
    long ActorId,
    NotificationType Type,
    bool IsRead,
    long? FollowId,
    long? PostId,
    long? CommentId,
    long? LiveStreamId,
    DateTime CreatedAt);
