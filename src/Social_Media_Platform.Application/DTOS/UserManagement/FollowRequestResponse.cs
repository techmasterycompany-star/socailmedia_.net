namespace Social_Media_Platform.Application.Common.DTOs.UserManagement;

public sealed record FollowRequestResponse(
    long Id,
    long UserId,
    string Username,
    string DisplayName,
    string? ProfilePhoto,
    DateTimeOffset CreatedAt);
