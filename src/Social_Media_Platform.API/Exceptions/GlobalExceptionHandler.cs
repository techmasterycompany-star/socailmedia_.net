using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Social_Media_Platform.Application.Common.Exceptions;

namespace Social_Media_Platform.API.Exceptions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted)
        {
            return false;
        }


        var (status, title, detail) = exception switch
        {
            KeyNotFoundException or NotFoundException =>
                (StatusCodes.Status404NotFound, "Resource Not Found", exception.Message ?? "The requested resource could not be found."),

            ValidationException =>
                (StatusCodes.Status400BadRequest, "Validation Error", exception.Message ?? "One or more validation errors occurred."),

            ArgumentException or BadRequestException =>
                (StatusCodes.Status400BadRequest, "Bad Request", exception.Message ?? "The request could not be processed."),

            UnauthorizedAccessException =>
                (StatusCodes.Status401Unauthorized, "Unauthorized", "Authentication is required to access this resource."),

            InvalidOperationException or ConflictException =>
                (StatusCodes.Status409Conflict, "Conflict", exception.Message ?? "The request conflicts with the current resource state."),

            _ =>(StatusCodes.Status500InternalServerError, "Internal Server Error",
                    "An unexpected error occurred.")
        };

        logger.LogError(
            exception,
            "Unhandled exception processing {RequestMethod} {RequestPath}.",
            httpContext.Request.Method,
            httpContext.Request.Path);

        var problemDetails = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        httpContext.Response.ContentType = "application/json";
        await httpContext.Response.WriteAsJsonAsync(
            new ExceptionApiResponse(false, problemDetails),
            cancellationToken);

        return true;
    }

    private sealed record ExceptionApiResponse(bool Success, ProblemDetails Error);
}
