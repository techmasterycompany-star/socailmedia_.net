using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.Common.DTOs.Admin;
using Social_Media_Platform.Application.Interfaces.IServices.Admin;

namespace Social_Media_Platform.API.Controllers.Admin
{
    [Route("api/v1/admin/users")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminUserService service;
        public AdminUserController(IAdminUserService service) => this.service = service;



        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<AdminUserListItemDto>>>> GetUsers()
        {
            var user = await service.GetUsersAsync();
            return Ok(ApiResponse < List < AdminUserListItemDto >>.SuccessResponse(user));
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<ApiResponse<AdminUserListItemDto>>> GetUser(long id)
        {
            var user = await service.GetUserAsync(id);

            return Ok(ApiResponse<AdminUserListItemDto>.SuccessResponse(user));
        }

        [HttpPut("{id:long}/status")]
        public async Task<ActionResult<ApiResponse<AdminUserListItemDto>>> UpdateStatus(long id,[FromBody] UpdateUserStatusRequest request)
        {
            var user = await service.UpdateUserStatusAsync(id, request);

            return Ok(ApiResponse<AdminUserListItemDto>.SuccessResponse(user));
        }

        [HttpPost("{id:long}/roles/moderator")]
        public async Task<ActionResult<ApiResponse<CommandResult>>> AssignModeratorRole(long id)
        {
            var result = await service.AssignModeratorRoleAsync(id);

            return Ok(ApiResponse<CommandResult>.SuccessResponse(result));
        }

        [HttpDelete("{id:long}/roles/moderator")]
        public async Task<ActionResult<ApiResponse<CommandResult>>> RemoveModeratorRole(long id)
        {
            var result = await service.RemoveModeratorRoleAsync(id);

            return Ok(ApiResponse<CommandResult>.SuccessResponse(result));
        }

    }
}
