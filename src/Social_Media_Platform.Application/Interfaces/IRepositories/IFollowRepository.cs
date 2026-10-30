using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.Common.DTOs.UserManagement;

namespace Social_Media_Platform.Application.Interfaces.IRepositories
{
    public interface IFollowRepository
    {
        Task<bool> IsFollowingAsync(
            long followerId,
            long followingId,
            CancellationToken cancellationToken = default);
        Task<bool> IsBlockedAsync(long firstUserId, long secondUserId);

        Task<UserProfileResponse> GetVisibleProfileAsync(
            long viewerId,
            long userId,
            CancellationToken cancellationToken);

        Task<PagedResult<UserProfileResponse>> SearchVisibleProfilesAsync(
            long viewerId,
            string query,
            int page,
            int pageSize,
            CancellationToken cancellationToken);

        Task<UserRelationshipResponse> FollowAsync(
            long followerId,
            long followingId,
            CancellationToken cancellationToken);

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

        Task RemoveRelationshipsAsync(long firstUserId, long secondUserId, CancellationToken cancellationToken);

        Task<IReadOnlyList<long>> GetAcceptedFollowerIdsAsync(long followingId,
            CancellationToken cancellationToken = default);
    }
}