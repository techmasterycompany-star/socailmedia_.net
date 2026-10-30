using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.API.Helpers;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Application.Interfaces.IServices;

namespace Social_Media_Platform.API.Controllers.Posts
{
    [ApiController]
    [Route("api/v1/feed")]
    [Authorize]
    public sealed class FeedController(IPostService postService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await postService.GetFeedAsync(User.GetAuthenticatedUserId(), page, pageSize, cancellationToken);
            return Ok(ApiResponse<PagedResult<PostDto>>.SuccessResponse(result));
        }

        [HttpGet("discovery")]
        public async Task<IActionResult> GetDiscoveryFeed([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await postService.GetDiscoveryFeedAsync(User.GetAuthenticatedUserId(), page, pageSize, cancellationToken);
            return Ok(ApiResponse<PagedResult<PostDto>>.SuccessResponse(result));
        }

    }
}
