using Livekit.Server.Sdk.Dotnet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.Infrastructure.Services.LiveKit;

namespace Social_Media_Platform.API.Controllers.LiveStreaming;

[ApiController]
[Route("api/livekit/webhook")]
public sealed class LiveKitWebhookController : ControllerBase
{
    private readonly WebhookReceiver _webhookReceiver;
    private readonly HandleLiveKitWebhook _handler;
    private readonly ILogger<LiveKitWebhookController> _logger;

    public LiveKitWebhookController(WebhookReceiver webhookReceiver, HandleLiveKitWebhook handler, ILogger<LiveKitWebhookController> logger)
    {
        _webhookReceiver = webhookReceiver;
        _handler = handler;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Receive()
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();

        var authorization = Request.Headers.Authorization.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(authorization))
            return Unauthorized();

        WebhookEvent webhookEvent;

        try
        {
            webhookEvent = _webhookReceiver.Receive(body, authorization);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Invalid LiveKit webhook signature.");

            return Unauthorized();
        }

        if (webhookEvent.Event is not ("room_started" or "room_finished"))
        {
            return Ok();
        }

        var roomName = webhookEvent.Room?.Name;

        if (string.IsNullOrWhiteSpace(roomName))
            return BadRequest();

        await _handler.HandleRoomStatusAsync(webhookEvent.Event, roomName);

        return Ok();
    }
}