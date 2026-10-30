using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.API.Helpers;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Application.Interfaces.IServices;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.API.Controllers.Posts
{

    [ApiController]
    [Route("api/v1/posts")]
    [Authorize]
    public sealed class PostsController(IPostService postService) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "User")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(210L * 1024 * 1024)]
        public async Task<IActionResult> Create([FromForm] CreatePostRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetAuthenticatedUserId();
            var post = await postService.CreateAsync(userId, request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<PostDto>.SuccessResponse(post));
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
        {
            var post = await postService.GetByIdAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<PostDto>.SuccessResponse(post));
        }

        [HttpGet("author/{authorId:long}")]
        public async Task<IActionResult> GetAuthorPosts(long authorId,[FromQuery] int page = 1,[FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await postService.GetAuthorPostsAsync(
                authorId, User.GetAuthenticatedUserId(), page, pageSize, cancellationToken);

            return Ok(ApiResponse<PagedResult<PostDto>>.SuccessResponse(result));
        }

        [HttpGet("me")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetMyPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var userId = User.GetAuthenticatedUserId();
            var result = await postService.GetAuthorPostsAsync(userId, userId, page, pageSize, cancellationToken);

            return Ok(ApiResponse<PagedResult<PostDto>>.SuccessResponse(result));
        }

        [HttpPut("{id:long}")]
        [Authorize(Roles = "User")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(210L * 1024 * 1024)]
        public async Task<IActionResult> Update(long id, [FromForm] UpdatePostRequest request, CancellationToken cancellationToken)
        {
            var post = await postService.UpdateAsync(id, User.GetAuthenticatedUserId(), request, cancellationToken);
            return Ok(ApiResponse<PostDto>.SuccessResponse(post));
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            await postService.DeleteAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<object>.SuccessResponse(new { message = "Post deleted successfully." }));
        }

        [HttpGet("{id:long}/share-link")]
        public async Task<IActionResult> GetShareLink(long id, CancellationToken cancellationToken)
        {
            var result = await postService.GetShareLinkAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<ShareLinkDto>.SuccessResponse(result));
        }

        [HttpGet("saved")]
        public async Task<IActionResult> GetSaved([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await postService.GetSavedAsync(User.GetAuthenticatedUserId(), page, pageSize, cancellationToken);
            return Ok(ApiResponse<PagedResult<PostDto>>.SuccessResponse(result));
        }
    }
}
