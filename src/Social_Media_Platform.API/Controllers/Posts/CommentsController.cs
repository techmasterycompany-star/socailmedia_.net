using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.API.Helpers;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.Interfaces.IServices;

namespace Social_Media_Platform.API.Controllers.Posts
{
    [ApiController]
    [Route("api/v1/comments")]
    [Authorize]
    public sealed class CommentsController(IPostInteractionService interactionService) : ControllerBase
    {
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            await interactionService.DeleteCommentAsync(id, User.GetAuthenticatedUserId(), cancellationToken);
            return Ok(ApiResponse<object>.SuccessResponse(new { message = "Comment deleted successfully." }));
        }
    }
}
