using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Social_Media_Platform.API.Hubs;

[Authorize]
public sealed class NotificationHub : Hub
{
}
