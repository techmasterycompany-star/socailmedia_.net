using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.DTOs.Notifications;

public class NotificationCreateRequest
{
        public long recipientId;
        public long actorId;
        public NotificationType type;
        public long? followId = null;
        public long? postId = null;
        public long? commentId = null;
        public long? liveStreamId = null;
};