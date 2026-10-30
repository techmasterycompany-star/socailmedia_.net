using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.API.Helpers;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOS.LiveStream;
using Social_Media_Platform.Application.DTOS.LiveStreaming;
using Social_Media_Platform.Application.Interfaces.IServices;

namespace Social_Media_Platform.API.Controllers.LiveStreaming;

[ApiController]
[Route("livestream")]
[Authorize]
public class LiveStreamController : ControllerBase
{
    private readonly ILiveStreamService _liveStreamService;
    public LiveStreamController(ILiveStreamService liveStreamService)
    {
        _liveStreamService = liveStreamService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var liveStreams = await _liveStreamService.GetAllAsync(page, pageSize);
        return Ok(ApiResponse<PagedResult<LiveStreamDto>>.SuccessResponse(liveStreams));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var liveStream = await _liveStreamService.ExecuteGetByIdAsync(id);
        return Ok(ApiResponse<LiveStreamDto>.SuccessResponse(liveStream));
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start([FromBody] StartLiveStreamRequest request)
    {
        var hostId = User.GetAuthenticatedUserId();

        var response = await _liveStreamService.ExecuteStartAsync(hostId, request);

        return Ok(ApiResponse<StartLiveStreamResponse>.SuccessResponse(response));
    }

    [HttpPost("{streamId:long}/join")]
    public async Task<IActionResult> Join(long streamId)
    {
        var viewerId = User.GetAuthenticatedUserId();

        var response = await _liveStreamService.ExecuteJoinAsync(viewerId, streamId);

        return Ok(ApiResponse<StartLiveStreamResponse>.SuccessResponse(response));
    }

    [HttpPost("{streamId:long}/end")]
    public async Task<IActionResult> End(long streamId)
    {
        var hostId = User.GetAuthenticatedUserId();

        await _liveStreamService.ExecuteEndAsync(hostId, streamId);

        string response = "The request to end the live stream was processed.";

        return Ok(ApiResponse<string>.SuccessResponse(response));
    }

    [HttpGet("{streamId:long}/messages")]
    public async Task<IActionResult> GetMessages(long streamId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var hostId = User.GetAuthenticatedUserId();
        var messages = await _liveStreamService.ExecuteGetMessagesAsync(hostId, streamId, page, pageSize);
        return Ok(ApiResponse<PagedResult<StreamChatMessageDTO>>.SuccessResponse(messages));
    }
}