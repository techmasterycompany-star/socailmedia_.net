using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Application.Interfaces.IServices;

namespace Social_Media_Platform.API.Controllers.Posts
{
    [ApiController]
    [Route("api/v1/hashtags")]
    [Authorize]
    public sealed class HashtagsController(ISearchService searchService) : ControllerBase
    {
        [HttpGet("trending")]
        public async Task<IActionResult> GetTrending([FromQuery] int limit = 10, CancellationToken cancellationToken = default)
        {
            var result = await searchService.GetTrendingHashtagsAsync(limit, cancellationToken);
            return Ok(ApiResponse<List<TrendingHashtagDto>>.SuccessResponse(result));
        }
    }
}
