using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Social_Media_Platform.Application.Common.DTOs;

namespace Social_Media_Platform.API.Controllers;

[ApiController]
[Route("health")]
public sealed class HealthController(HealthCheckService healthCheckService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<HealthCheckResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<HealthCheckResponse>), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ApiResponse<HealthCheckResponse>>> Get(
        CancellationToken cancellationToken)
    {
        var report = await healthCheckService.CheckHealthAsync(cancellationToken);
        var response = new HealthCheckResponse(report.Status.ToString());

        return report.Status == HealthStatus.Healthy
            ? Ok(ApiResponse<HealthCheckResponse>.SuccessResponse(response))
            : StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                ApiResponse<HealthCheckResponse>.FailResponse(response));
    }
}

public sealed record HealthCheckResponse(string Status);
