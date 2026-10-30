using Microsoft.AspNetCore.SignalR;
using Social_Media_Platform.Application.DTOs.Notifications;
using Social_Media_Platform.Application.Interfaces.IServices;

namespace Social_Media_Platform.API.Hubs;

public sealed class SignalRNotificationPublisher : INotificationPublisher
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationPublisher(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }
    public Task PublishAsync(long recipientId,NotificationResponse notification) =>
        _hubContext.Clients.User(recipientId.ToString())
            .SendAsync("ReceiveNotification", notification);
}
