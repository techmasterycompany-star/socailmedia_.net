using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.API.Helpers;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Application.Interfaces.IServices;

namespace Social_Media_Platform.API.Controllers.Posts
{
    [ApiController]
    [Route("api/v1/posts/{id:long}")]
    [Authorize]
    public sealed class PostInteractionsController(IPostInteractionService interactionService) : ControllerBase
    {
        [HttpPost("likes")]
        public async Task<IActionResult> LikePost(long id, CancellationToken cancellationToken)
        {
            var result = await interactionService.LikePostAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<LikeStateDto>.SuccessResponse(result));
        }

        [HttpDelete("likes")]
        public async Task<IActionResult> UnlikePost(long id, CancellationToken cancellationToken)
        {
            var result = await interactionService.UnlikePostAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<LikeStateDto>.SuccessResponse(result));
        }

        [HttpGet("comments")]
        public async Task<IActionResult> GetComments(long id, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken cancellationToken = default)
        {
            var result = await interactionService.GetCommentsAsync(id, User.GetAuthenticatedUserId(), page, pageSize, cancellationToken);
            return Ok(ApiResponse<PagedResult<CommentDto>>.SuccessResponse(result));
        }

        [HttpPost("comments")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> CreateComment(long id, [FromBody] CreateCommentRequest request, CancellationToken cancellationToken)
        {
            var result = await interactionService.CreateCommentAsync(id, User.GetAuthenticatedUserId(), request, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<CommentDto>.SuccessResponse(result));
        }

        [HttpPost("save")]
        public async Task<IActionResult> SavePost(long id, CancellationToken cancellationToken)
        {
            var result = await interactionService.SavePostAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<SaveStateDto>.SuccessResponse(result));
        }

        [HttpDelete("unsave")]
        public async Task<IActionResult> UnsavePost(long id, CancellationToken cancellationToken)
        {
            var result = await interactionService.UnsavePostAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<SaveStateDto>.SuccessResponse(result));
        }
    }
}
