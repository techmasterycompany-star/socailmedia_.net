namespace Social_Media_Platform.Application.Common.DTOs.UserManagement;

public sealed record UserProfileResponse(
    long Id,
    string Username,
    string DisplayName,
    string? Bio,
    string? ProfilePhoto,
    bool IsPrivate,
    int FollowerCount,
    int FollowingCount,
    bool IsFollowing,
    bool IsFollowedBy,
    bool HasPendingRequest
);
