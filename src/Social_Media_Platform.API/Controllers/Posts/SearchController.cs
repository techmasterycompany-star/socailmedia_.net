using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.API.Helpers;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Application.Interfaces.IServices;

namespace Social_Media_Platform.API.Controllers.Posts
{
    [ApiController]
    [Route("api/v1/search")]
    [Authorize]
    public sealed class SearchController(ISearchService searchService) : ControllerBase
    {
        [HttpGet("posts")]
        public async Task<IActionResult> SearchPosts([FromQuery] SearchPostsRequest request, CancellationToken cancellationToken)
        {
            var result = await searchService.SearchPostsAsync(request.Q, request.Hashtag, User.GetAuthenticatedUserId(), request.Page, request.PageSize, cancellationToken);
            return Ok(ApiResponse<PagedResult<PostDto>>.SuccessResponse(result));
        }

        [HttpGet("hashtags")]
        public async Task<IActionResult> SearchHashtags([FromQuery] SearchHashtagsRequest request, CancellationToken cancellationToken)
        {
            var result = await searchService.SearchHashtagsAsync(request.Q, request.Limit, cancellationToken);
            return Ok(ApiResponse<List<HashtagDto>>.SuccessResponse(result));
        }

        [HttpGet("users")]
        public async Task<IActionResult> SearchUsers([FromQuery] SearchUsersRequest request, CancellationToken cancellationToken)
        {
            var result = await searchService.SearchUsersAsync(request.Q, User.GetAuthenticatedUserId(), request.Page, request.PageSize, cancellationToken);
            return Ok(ApiResponse<PagedResult<UserSummaryDto>>.SuccessResponse(result));
        }
    }
}
