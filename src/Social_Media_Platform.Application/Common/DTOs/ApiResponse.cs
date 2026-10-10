namespace Social_Media_Platform.Application.Common.DTOs;

public sealed class ApiResponse<T>
{
    public bool Success { get; init; }

    public T? Data { get; init; }

    public static ApiResponse<T> SuccessResponse(T data) => new()
    {
        Success = true,
        Data = data
    };

    public static ApiResponse<T> FailResponse(T data) => new()
    {
        Success = false,
        Data = data
    };
}
