using Social_Media_Platform.Application.DTOs.Notifications;

namespace Social_Media_Platform.Application.Interfaces.IServices;

public interface INotificationPublisher
{
    Task PublishAsync(long recipientId, NotificationResponse notification);
}
