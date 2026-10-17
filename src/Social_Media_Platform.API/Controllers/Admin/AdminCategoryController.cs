using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.Common.DTOs.Admin;
using Social_Media_Platform.Application.Interfaces.IServices.Admin;
using System.Security.Claims;

namespace Social_Media_Platform.API.Controllers.Admin
{
    [Route("api/v1/admin/categories")]
    [ApiController]
    public class AdminCategoryController : ControllerBase
    {
        private readonly IAdminCategoryService service;
        public AdminCategoryController(IAdminCategoryService service) => this.service = service;



        [HttpGet]
        [Authorize(Roles = "Moderator,Admin")]
        public async Task<ActionResult<ApiResponse<List<CategoryDto>>>> GetCategories()
        {
            var result = await service.GetCategoriesAsync();

            return Ok(ApiResponse<List<CategoryDto>>.SuccessResponse(result));
        }

        [HttpGet("{id:long}")]
        [Authorize(Roles = "Moderator,Admin")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategory(long id)
        {
            var category = await service.GetCategoryAsync(id);

            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            var category = await service.CreateCategoryAsync(request, CurrentUserId);

            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<CategoryDto>.SuccessResponse(category));
        }

        [HttpPut("{id:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(long id,[FromBody] UpdateCategoryRequest request)
        {
            var category = await service.UpdateCategoryAsync(id, request, CurrentUserId);

            return Ok(ApiResponse<CategoryDto>.SuccessResponse(category));
        }

        [HttpDelete("{id:long}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<CommandResult>>> DeleteCategory(long id)
        {
            var result = await service.DeleteCategoryAsync(id, CurrentUserId);

            return Ok(ApiResponse<CommandResult>.SuccessResponse(result));
        }



        protected long? CurrentUserId =>
             long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;
    }
}
