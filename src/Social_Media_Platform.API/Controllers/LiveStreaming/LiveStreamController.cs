using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.API.Helpers;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOS.LiveStream;
using Social_Media_Platform.Application.Interfaces.IServices;

namespace Social_Media_Platform.API.Controllers.LiveStreaming;

[ApiController]
[Route("livestream")]
public class LiveStreamController : ControllerBase
{
    private readonly ILiveStreamService _liveStreamService;
    public LiveStreamController(ILiveStreamService liveStreamService)
    {
        _liveStreamService = liveStreamService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var liveStreams = await _liveStreamService.GetAllAsync();
        return Ok(ApiResponse<IEnumerable<LiveStreamDto>>.SuccessResponse(liveStreams));
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] StartLiveStreamRequest request)
    {
        var hostId = User.GetAuthenticatedUserId();
        // long hostId = 1;

        var response = await _liveStreamService.ExecuteStartAsync(hostId, request);

        return Ok(ApiResponse<StartLiveStreamResponse>.SuccessResponse(response));
    }

    [HttpPost("join/{streamId}")]
    public async Task<IActionResult> Join(long streamId)
    {
        var viewerId = User.GetAuthenticatedUserId();
        // long viewerId = 1;

        var response = await _liveStreamService.ExecuteJoinAsync(viewerId, streamId);

        return Ok(ApiResponse<StartLiveStreamResponse>.SuccessResponse(response));
    }

    [HttpPost("end/{streamId}")]
    public async Task<IActionResult> End(long streamId)
    {
        var hostId = User.GetAuthenticatedUserId();
        // long hostId = 1;

        await _liveStreamService.ExecuteEndAsync(hostId, streamId);

        string response = "The request to end the live stream was processed.";

        return Ok(ApiResponse<string>.SuccessResponse(response));
    }
}