using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOS.Moderator;
using Social_Media_Platform.Application.Interfaces.IServices.Moderator;
using System.Security.Claims;

namespace Social_Media_Platform.API.Controllers.Admin
{
    [Route("api/v1/moderation")]
    [ApiController]
    [Authorize(Roles = "Moderator,Admin")]
    public class ModerationController : ControllerBase
    {
        private readonly IModerationService service;
        public ModerationController(IModerationService service) => this.service = service;



        [HttpDelete("posts/{id:long}")]
        public async Task<ActionResult<ApiResponse<ModerationActionResponse>>> HidePost(long id,[FromQuery] string? reason)
        {
            var result = await service.HidePostAsync(id, reason, CurrentUserId);

            return Ok(ApiResponse<ModerationActionResponse>.SuccessResponse(result));
        }

        [HttpDelete("comments/{id:long}")]
        public async Task<ActionResult<ApiResponse<ModerationActionResponse>>> RemoveComment(long id,[FromQuery] string? reason)
        {
            var result = await service.RemoveCommentAsync(id, reason, CurrentUserId);

            return Ok(ApiResponse<ModerationActionResponse>.SuccessResponse(result));
        }

        [HttpPost("streams/{id:long}/stop")]
        public async Task<ActionResult<ApiResponse<ModerationActionResponse>>> StopStream(long id,[FromQuery] string? reason)
        {
            var result = await service.StopStreamAsync(id, reason, CurrentUserId);

            return Ok(ApiResponse<ModerationActionResponse>.SuccessResponse(result));
        }


        protected long? CurrentUserId =>
               long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;


    }
}
