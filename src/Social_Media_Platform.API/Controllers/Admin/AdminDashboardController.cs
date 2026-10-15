using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.Common.DTOs.Admin;
using Social_Media_Platform.Application.Interfaces.IServices.Admin;

namespace Social_Media_Platform.API.Controllers.Admin
{
    [ApiController]
    [Route("api/v1/admin/dashboard")]
    [Authorize(Roles ="Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardService service;
        public AdminDashboardController(IAdminDashboardService service) => this.service = service;


        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<AdminDashboardStatsDto>>> GetStats()
        {
            var stats = await service.GetStatsAsync();

            return Ok(ApiResponse<AdminDashboardStatsDto>.SuccessResponse(stats));
        }
    }
}
