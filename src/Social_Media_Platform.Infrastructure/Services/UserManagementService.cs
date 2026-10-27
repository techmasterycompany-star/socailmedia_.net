using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.Common.DTOs.UserManagement;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Application.Interfaces.IServices;

namespace Social_Media_Platform.Infrastructure.Services;

public sealed class UserManagementService : IUserManagementService
{
    private readonly IUserManagementRepository _userManagementRepository;
    private readonly IFollowRepository _followRepository;

    public UserManagementService(IUserManagementRepository userManagementRepository, IFollowRepository followRepository)
    {
        _userManagementRepository = userManagementRepository;
        _followRepository = followRepository;
    }
    public Task<UserProfileResponse> GetProfileAsync(long viewerId, long userId, CancellationToken cancellationToken)
    {
        return _followRepository.GetVisibleProfileAsync(viewerId, userId, cancellationToken);

    }

    public Task<UserProfileResponse> UpdateProfileAsync(
        long userId,
        UpdateUserProfileRequest request,
        CancellationToken cancellationToken) =>
        _userManagementRepository.UpdateProfileAsync(userId, request, cancellationToken);

    public Task<UserRelationshipResponse> FollowAsync(
        long followerId,
        long followingId,
        CancellationToken cancellationToken) =>
        _followRepository.FollowAsync(followerId, followingId, cancellationToken);

    public Task UnfollowAsync(
        long followerId,
        long followingId,
        CancellationToken cancellationToken) =>
        _followRepository.UnfollowAsync(followerId, followingId, cancellationToken);

    public Task ApproveFollowRequestAsync(
        long recipientId,
        long requestId,
        CancellationToken cancellationToken) =>
        _followRepository.ApproveFollowRequestAsync(recipientId, requestId, cancellationToken);

    public Task RejectFollowRequestAsync(
        long recipientId,
        long requestId,
        CancellationToken cancellationToken) =>
        _followRepository.RejectFollowRequestAsync(recipientId, requestId, cancellationToken);

    public Task CancelFollowRequestAsync(
        long followerId,
        long requestId,
        CancellationToken cancellationToken) =>
        _followRepository.CancelFollowRequestAsync(followerId, requestId, cancellationToken);

    public Task<PagedResult<FollowRequestResponse>> GetFollowRequestsAsync(
        long userId,
        bool incoming,
        int page,
        int pageSize,
        CancellationToken cancellationToken) =>
        _followRepository.GetFollowRequestsAsync(userId, incoming, page, pageSize, cancellationToken);

    public Task BlockAsync(long blockerId, long blockedId, CancellationToken cancellationToken) =>
        _userManagementRepository.BlockAsync(blockerId, blockedId, cancellationToken);

    public Task UnblockAsync(long blockerId, long blockedId, CancellationToken cancellationToken) =>
        _userManagementRepository.UnblockAsync(blockerId, blockedId, cancellationToken);

    public Task<PagedResult<UserProfileResponse>> SearchAsync(
        long viewerId,
        string query,
        int page,
        int pageSize,
        CancellationToken cancellationToken) =>
        _followRepository.SearchVisibleProfilesAsync(viewerId, query, page, pageSize, cancellationToken);

    public Task<PagedResult<UserProfileResponse>> GetFollowersAsync(
        long viewerId,
        long userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken) =>
        _followRepository.GetFollowersAsync(viewerId, userId, page, pageSize, cancellationToken);

    public Task<PagedResult<UserProfileResponse>> GetFollowingAsync(
        long viewerId,
        long userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken) =>
        _followRepository.GetFollowingAsync(viewerId, userId, page, pageSize, cancellationToken);

    public Task<UserRelationshipResponse> GetRelationshipAsync(
        long viewerId,
        long userId,
        CancellationToken cancellationToken) =>
        _followRepository.GetRelationshipAsync(viewerId, userId, cancellationToken);
}
