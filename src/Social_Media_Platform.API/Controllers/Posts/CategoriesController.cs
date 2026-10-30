using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Application.Interfaces.IServices;

namespace Social_Media_Platform.API.Controllers.Posts
{
    [ApiController]
    [Route("api/v1/categories")]
    [Authorize]
    public sealed class CategoriesController(ISearchService searchService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await searchService.GetCategoriesAsync(cancellationToken);
            return Ok(ApiResponse<List<CategoryDto>>.SuccessResponse(result));
        }
    }
}
