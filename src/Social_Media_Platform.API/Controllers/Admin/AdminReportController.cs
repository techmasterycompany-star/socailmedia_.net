using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.Common.DTOs.Moderator;
using Social_Media_Platform.Application.Interfaces.IServices.Moderator;
using System.Security.Claims;

namespace Social_Media_Platform.API.Controllers.Admin
{
    [Route("api/v1/reports")]
    [ApiController]
    [Authorize]
    public class AdminReportController : ControllerBase
    {
        private readonly IReportService service;
        public AdminReportController(IReportService service) => this.service = service;




        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ReportDetailDto>>> CreateReport([FromBody] CreateReportRequest request)
        {
            var reporterId = RequireUserId();
            var report = await service.CreateReportAsync(reporterId, request);

            return StatusCode(
                StatusCodes.Status201Created,
                ApiResponse<ReportDetailDto>.SuccessResponse(report));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ApiResponse<List<ReportListItemDto>>>> GetReports()
        {
            var result = await service.GetReportsAsync();

            return Ok(ApiResponse<List<ReportListItemDto>>.SuccessResponse(result));
        }

        [HttpGet("{id:long}")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ApiResponse<ReportDetailDto>>> GetReport(long id)
        {
            var report = await service.GetReportAsync(id);

            return Ok(ApiResponse<ReportDetailDto>.SuccessResponse(report));
        }

        [HttpPost("{id:long}/review")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ApiResponse<ReportDetailDto>>> StartReview(long id,[FromQuery] string? notes)
        {
            var report = await service.StartReviewAsync(id, CurrentUserId, notes);

            return Ok(ApiResponse<ReportDetailDto>.SuccessResponse(report));
        }

        [HttpPost("{id:long}/resolve")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ApiResponse<ReportDetailDto>>> ResolveReport(long id,[FromBody] ResolveReportRequest request)
        {
            var report = await service.ResolveReportAsync(id, CurrentUserId, request);

            return Ok(ApiResponse<ReportDetailDto>.SuccessResponse(report));
        }

        [HttpPost("{id:long}/dismiss")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ApiResponse<ReportDetailDto>>> DismissReport(long id,[FromQuery] string? reason)        
        {
            var report = await service.DismissReportAsync(id, CurrentUserId, reason);

            return Ok(ApiResponse<ReportDetailDto>.SuccessResponse(report));
        }

        [HttpPost("{id:long}/notes")]
        [Authorize(Roles = "Admin,Moderator")]
        public async Task<ActionResult<ApiResponse<ReportDetailDto>>> AddModeratorNote(long id,[FromBody] AddModeratorNoteRequest request)
        {
            var report = await service.AddModeratorNoteAsync(id, CurrentUserId, request);

            return Ok(ApiResponse<ReportDetailDto>.SuccessResponse(report));
        }








        protected long? CurrentUserId =>
              long.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : null;

        protected long RequireUserId() =>
            CurrentUserId ?? throw new UnauthorizedAccessException(
                "An authenticated user is required to perform this action");
    }
}
