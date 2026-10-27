namespace Social_Media_Platform.Application.Common.DTOs.UserManagement;

public sealed record UserRelationshipResponse(
    long UserId,
    bool IsFollowing,
    bool IsFollowedBy,
    bool HasPendingRequest,
    int FollowerCount,
    int FollowingCount,
    int IncomingPendingRequestCount,
    int OutgoingPendingRequestCount);
