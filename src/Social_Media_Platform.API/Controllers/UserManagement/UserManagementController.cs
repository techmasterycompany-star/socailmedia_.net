using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.API.Helpers;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.Common.DTOs.UserManagement;
using Social_Media_Platform.Application.Interfaces.IServices;

namespace Social_Media_Platform.API.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/users")]
public sealed class UserManagementController : ControllerBase
{
    private readonly IUserManagementService _userManagementService;
    public UserManagementController(IUserManagementService userManagementService)
    {
        _userManagementService = userManagementService;
    }

    [HttpGet("{userId:long}")]
    public async Task<ActionResult<ApiResponse<UserProfileResponse>>> GetProfile(long userId, CancellationToken cancellationToken)
    {
        var profile = await _userManagementService.GetProfileAsync(User.GetAuthenticatedUserId(), userId, cancellationToken);
        return Ok(ApiResponse<UserProfileResponse>.SuccessResponse(profile));
    }

    [HttpPut("me/profile")]
    public async Task<ActionResult<ApiResponse<UserProfileResponse>>> UpdateProfile([FromBody] UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        var profile = await _userManagementService.UpdateProfileAsync(User.GetAuthenticatedUserId(), request, cancellationToken);
        return Ok(ApiResponse<UserProfileResponse>.SuccessResponse(profile));
    }

    [HttpPost("{userId:long}/follow")]
    public async Task<ActionResult<ApiResponse<UserRelationshipResponse>>> Follow(long userId, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.FollowAsync(User.GetAuthenticatedUserId(), userId, cancellationToken);
        return Ok(ApiResponse<UserRelationshipResponse>.SuccessResponse(result));
    }

    [HttpDelete("{userId:long}/follow")]
    public async Task<IActionResult> Unfollow(long userId, CancellationToken cancellationToken)
    {
        await _userManagementService.UnfollowAsync(User.GetAuthenticatedUserId(), userId, cancellationToken);
        return NoContent();
    }

    [HttpPost("follow-requests/{requestId:long}/approve")]
    public async Task<IActionResult> ApproveFollowRequest(long requestId, CancellationToken cancellationToken)
    {
        await _userManagementService.ApproveFollowRequestAsync(User.GetAuthenticatedUserId(), requestId, cancellationToken);
        return NoContent();
    }

    [HttpPost("follow-requests/{requestId:long}/reject")]
    public async Task<IActionResult> RejectFollowRequest(long requestId, CancellationToken cancellationToken)
    {
        await _userManagementService.RejectFollowRequestAsync(User.GetAuthenticatedUserId(), requestId, cancellationToken);
        return NoContent();
    }

    [HttpDelete("follow-requests/{requestId:long}")]
    public async Task<IActionResult> CancelFollowRequest(long requestId, CancellationToken cancellationToken)
    {
        await _userManagementService.CancelFollowRequestAsync(
            User.GetAuthenticatedUserId(),
            requestId,
            cancellationToken);
        return NoContent();
    }

    [HttpGet("follow-requests/incoming")]
    public async Task<ActionResult<ApiResponse<PagedResult<FollowRequestResponse>>>> GetIncomingFollowRequests(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var requests = await _userManagementService.GetFollowRequestsAsync(
            User.GetAuthenticatedUserId(),
            incoming: true,
            page,
            pageSize,
            cancellationToken);
        return Ok(ApiResponse<PagedResult<FollowRequestResponse>>.SuccessResponse(requests));
    }

    [HttpGet("follow-requests/outgoing")]
    public async Task<ActionResult<ApiResponse<PagedResult<FollowRequestResponse>>>> GetOutgoingFollowRequests(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var requests = await _userManagementService.GetFollowRequestsAsync(
            User.GetAuthenticatedUserId(),
            incoming: false,
            page,
            pageSize,
            cancellationToken);
        return Ok(ApiResponse<PagedResult<FollowRequestResponse>>.SuccessResponse(requests));
    }

    [HttpPost("{userId:long}/block")]
    public async Task<IActionResult> Block(long userId, CancellationToken cancellationToken)
    {
        await _userManagementService.BlockAsync(
            User.GetAuthenticatedUserId(),
            userId,
            cancellationToken);
        return NoContent();
    }

    [HttpDelete("{userId:long}/block")]
    public async Task<IActionResult> Unblock(long userId, CancellationToken cancellationToken)
    {
        await _userManagementService.UnblockAsync(
            User.GetAuthenticatedUserId(),
            userId,
            cancellationToken);
        return NoContent();
    }

    [HttpGet("search")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserProfileResponse>>>> Search(
        [FromQuery] string query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _userManagementService.SearchAsync(
            User.GetAuthenticatedUserId(),
            query,
            page,
            pageSize,
            cancellationToken);
        return Ok(ApiResponse<PagedResult<UserProfileResponse>>.SuccessResponse(result));
    }

    [HttpGet("{userId:long}/followers")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserProfileResponse>>>> GetFollowers(
        long userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _userManagementService.GetFollowersAsync(
            User.GetAuthenticatedUserId(),
            userId,
            page,
            pageSize,
            cancellationToken);
        return Ok(ApiResponse<PagedResult<UserProfileResponse>>.SuccessResponse(result));
    }

    [HttpGet("{userId:long}/following")]
    public async Task<ActionResult<ApiResponse<PagedResult<UserProfileResponse>>>> GetFollowing(
        long userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _userManagementService.GetFollowingAsync(
            User.GetAuthenticatedUserId(),
            userId,
            page,
            pageSize,
            cancellationToken);
        return Ok(ApiResponse<PagedResult<UserProfileResponse>>.SuccessResponse(result));
    }

    [HttpGet("{userId:long}/relationship")]
    public async Task<ActionResult<ApiResponse<UserRelationshipResponse>>> GetRelationship(long userId, CancellationToken cancellationToken)
    {
        var result = await _userManagementService.GetRelationshipAsync(
            User.GetAuthenticatedUserId(),
            userId,
            cancellationToken);
        return Ok(ApiResponse<UserRelationshipResponse>.SuccessResponse(result));
    }
}
