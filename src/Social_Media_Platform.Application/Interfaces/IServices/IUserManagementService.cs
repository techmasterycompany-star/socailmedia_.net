using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.Common.DTOs.UserManagement;

namespace Social_Media_Platform.Application.Interfaces.IServices;

public interface IUserManagementService
{
    Task<UserProfileResponse> GetProfileAsync(long viewerId, long userId, CancellationToken cancellationToken);

    Task<UserProfileResponse> UpdateProfileAsync(long userId, UpdateUserProfileRequest request, CancellationToken cancellationToken);

    Task<UserRelationshipResponse> FollowAsync(long followerId, long followingId, CancellationToken cancellationToken);

    Task UnfollowAsync(long followerId, long followingId, CancellationToken cancellationToken);

    Task ApproveFollowRequestAsync(long recipientId, long requestId, CancellationToken cancellationToken);

    Task RejectFollowRequestAsync(long recipientId, long requestId, CancellationToken cancellationToken);

    Task CancelFollowRequestAsync(long followerId, long requestId, CancellationToken cancellationToken);

    Task<PagedResult<FollowRequestResponse>> GetFollowRequestsAsync(
        long userId,
        bool incoming,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task BlockAsync(long blockerId, long blockedId, CancellationToken cancellationToken);

    Task UnblockAsync(long blockerId, long blockedId, CancellationToken cancellationToken);

    Task<PagedResult<UserProfileResponse>> SearchAsync(
        long viewerId,
        string query,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<PagedResult<UserProfileResponse>> GetFollowersAsync(
        long viewerId,
        long userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<PagedResult<UserProfileResponse>> GetFollowingAsync(
        long viewerId,
        long userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);

    Task<UserRelationshipResponse> GetRelationshipAsync(
        long viewerId,
        long userId,
        CancellationToken cancellationToken);
}
