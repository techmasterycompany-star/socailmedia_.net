using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.API.Helpers;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOs.Notifications;
using Social_Media_Platform.Application.Interfaces.IServices;

namespace Social_Media_Platform.API.Controllers;

[ApiController]
[Route("api/notifications")]
[Authorize]
public sealed class NotificationController(INotificationService notificationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<NotificationResponse>>>> GetNotifications(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var notifications = await notificationService.GetForRecipientAsync(
            User.GetAuthenticatedUserId(),
            page,
            pageSize);

        return Ok(ApiResponse<PagedResult<NotificationResponse>>.SuccessResponse(notifications));
    }

    [HttpGet("unread-count")]
    public async Task<ActionResult<ApiResponse<int>>> GetUnreadCount()
    {
        var unreadCount = await notificationService.GetUnreadCountAsync(
            User.GetAuthenticatedUserId());

        return Ok(ApiResponse<int>.SuccessResponse(unreadCount));
    }

    [HttpPatch("{id:long}/read")]
    public async Task<ActionResult<ApiResponse<NotificationResponse>>> MarkAsRead(
        long id)
    {
        var notification = await notificationService.MarkAsReadAsync(
            User.GetAuthenticatedUserId(),
            id);

        return Ok(ApiResponse<NotificationResponse>.SuccessResponse(notification));
    }

    [HttpPatch("read-all")]
    public async Task<ActionResult<ApiResponse<int>>> MarkAllAsRead()
    {
        var updatedCount = await notificationService.MarkAllAsReadAsync(
            User.GetAuthenticatedUserId());

        return Ok(ApiResponse<int>.SuccessResponse(updatedCount));
    }
}
