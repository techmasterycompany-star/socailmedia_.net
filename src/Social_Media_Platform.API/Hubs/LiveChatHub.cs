using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Social_Media_Platform.Application.Interfaces.IServices;
using Social_Media_Platform.Application.Interfaces.IServices.LiveStreaming;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.API.Hubs;

[Authorize]
public sealed class LiveChatHub : Hub
{
    private readonly ILiveStreamService _liveStreamService;
    private readonly ILiveStreamAccessService _liveStreamAccessService;

    public LiveChatHub(ILiveStreamService liveStreamService, ILiveStreamAccessService liveStreamAccessService)
    {
        _liveStreamService = liveStreamService;
        _liveStreamAccessService = liveStreamAccessService;
    }

    public async Task JoinStream(long streamId)
    {
        var senderId = GetUserId();

        var stream = await _liveStreamAccessService.ValidateCanJoinAsync(senderId, streamId);

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            GetGroupName(streamId));
    }

    public async Task SendMessage(long streamId, string content)
    {
        var senderId = GetUserId();

        var message = await _liveStreamService.ExecuteSendMessageAsync(
            senderId,
            streamId,
            content,
            StreamMessageType.Chat);

        await Clients.Group(GetGroupName(streamId))
            .SendAsync("ReceiveMessage", message);
    }

    public async Task SendReaction(long streamId, string content)
    {
        var userId = GetUserId();

        var reaction = await _liveStreamService.ExecuteSendMessageAsync(
            userId,
            streamId,
            content,
            StreamMessageType.Reaction);

        await Clients.Group(GetGroupName(streamId)).SendAsync("ReceiveReaction", reaction);
    }

    public async Task LeaveStream(long streamId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            GetGroupName(streamId));
    }

    private long GetUserId()
    {
        var userId =
            Context.User?.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? Context.User?.FindFirstValue("sub");

        if (!long.TryParse(userId, out var parsedUserId)
            || parsedUserId <= 0)
        {
            throw new HubException("Invalid authenticated user.");
        }

        return parsedUserId;
    }

    private static string GetGroupName(long streamId)
        => $"live-stream:{streamId}";
}
