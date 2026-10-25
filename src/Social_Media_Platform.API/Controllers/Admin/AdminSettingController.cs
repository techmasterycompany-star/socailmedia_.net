using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOS.Admin;
using Social_Media_Platform.Application.Interfaces.IServices.Admin;
using System.Security.Claims;

namespace Social_Media_Platform.API.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/settings")]
    [Authorize(Roles = "Admin")]
    public class AdminSettingController : ControllerBase
    {
        private readonly IAdminSettingsService service;
        public AdminSettingController(IAdminSettingsService service) => this.service = service;




        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<PlatformSettingDto>>>> GetSettings()
        {
            var result = await service.GetSettingsAsync();

            return Ok(ApiResponse<List<PlatformSettingDto>>.SuccessResponse(result));
        }

        [HttpGet("{key}")]
        public async Task<ActionResult<ApiResponse<PlatformSettingDto>>> GetSetting(string key)
        {
            var setting = await service.GetSettingAsync(key);

            return Ok(ApiResponse<PlatformSettingDto>.SuccessResponse(setting));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PlatformSettingDto>>> CreateSetting([FromBody] CreateSettingRequest request)
        {
            var setting = await service.CreateSettingAsync(request, CurrentUserId);

            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<PlatformSettingDto>.SuccessResponse(setting));
        }

        [HttpPut("{key}")]
        public async Task<ActionResult<ApiResponse<PlatformSettingDto>>> UpdateSetting(string key,[FromBody] UpdateSettingRequest request)
        {
            var setting = await service.UpdateSettingAsync(key, request, CurrentUserId);

            return Ok(ApiResponse<PlatformSettingDto>.SuccessResponse(setting));
        }

        [HttpDelete("{key}")]
        public async Task<ActionResult<ApiResponse<CommandResult>>> DeleteSetting(string key)
        {
            var result = await service.DeleteSettingAsync(key, CurrentUserId);

            return Ok(ApiResponse<CommandResult>.SuccessResponse(result));
        }



        protected long? CurrentUserId =>
              long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;
    }
}
