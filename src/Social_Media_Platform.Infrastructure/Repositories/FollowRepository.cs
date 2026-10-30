using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.Common.DTOs.UserManagement;
using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.Infrastructure.Repositories;

public class FollowRepository : IFollowRepository
{
    private readonly Social_Media_PlatformDbContext _context;

    public FollowRepository(Social_Media_PlatformDbContext context)
    {
        _context = context;
    }

    public async Task<bool> IsFollowingAsync(long followerId, long followingId, CancellationToken cancellationToken = default)
    {
        return await _context.Follows.AnyAsync(f =>
            f.FollowerId == followerId
            && f.FollowingId == followingId
            && f.Status == FollowStatus.Accepted
            && f.Follower.Status != UserStatus.Blocked
            && f.Following.Status != UserStatus.Blocked
            && !_context.Blocks.Any(block =>
                (block.BlockerId == followerId && block.BlockedId == followingId)
                || (block.BlockerId == followingId && block.BlockedId == followerId)),
            cancellationToken);
    }

    public Task<bool> IsBlockedAsync(long firstUserId, long secondUserId) =>
        _context.Blocks.AnyAsync(block =>
            (block.BlockerId == firstUserId && block.BlockedId == secondUserId)
            || (block.BlockerId == secondUserId && block.BlockedId == firstUserId));

    public async Task<UserProfileResponse> GetVisibleProfileAsync(
        long viewerId,
        long userId,
        CancellationToken cancellationToken)
    {
        await EnsureProfileVisibleAsync(viewerId, userId, cancellationToken);
        var profiles = _context.Profiles.Where(profile => profile.UserId == userId);
        return await ProfileQuery(viewerId, profiles)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("The requested profile could not be found.");
    }

    public async Task<PagedResult<UserProfileResponse>> SearchVisibleProfilesAsync(
        long viewerId,
        string query,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        ValidatePage(page, pageSize);
        query = query.Trim();
        if (query.Length is < 2 or > 100)
        {
            throw new BadRequestException("Search query must contain between 2 and 100 characters.");
        }

        var profiles = _context.Profiles.Where(profile =>
            profile.UserId != viewerId
            && profile.User.Status != UserStatus.Blocked
            && (!profile.IsPrivate
                || _context.Follows.Any(follow => follow.FollowerId == viewerId
                    && follow.FollowingId == profile.UserId
                    && follow.Status == FollowStatus.Accepted))
            && !_context.Blocks.Any(block =>
                (block.BlockerId == viewerId && block.BlockedId == profile.UserId)
                || (block.BlockedId == viewerId && block.BlockerId == profile.UserId)));
        profiles = profiles.Where(profile =>
            profile.User.Username.Contains(query) || profile.DisplayName.Contains(query));

        var totalCount = await profiles.CountAsync(cancellationToken);
        var pagedProfiles = profiles
            .OrderBy(profile => profile.User.Username)
            .ThenBy(profile => profile.UserId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        var items = await ProfileQuery(viewerId, pagedProfiles)
            .ToListAsync(cancellationToken);

        return PagedResult<UserProfileResponse>.Create(items, page, pageSize, totalCount);
    }

    public async Task<IReadOnlyList<long>> GetAcceptedFollowerIdsAsync(
        long followingId,
        CancellationToken cancellationToken = default)
    {
        return await _context.Follows
            .Where(follow => follow.FollowingId == followingId
                && follow.Status == FollowStatus.Accepted
                && follow.Follower.Status != UserStatus.Blocked
                && follow.Following.Status != UserStatus.Blocked
                && !_context.Blocks.Any(block =>
                    (block.BlockerId == follow.FollowerId && block.BlockedId == followingId)
                    || (block.BlockerId == followingId && block.BlockedId == follow.FollowerId)))
            .Select(follow => follow.FollowerId)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    public async Task<UserRelationshipResponse> FollowAsync(long followerId, long followingId, CancellationToken cancellationToken)
    {
        ValidateDistinctUserIds(followerId, followingId);
        await using var transaction = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);
        await EnsureUsersExistAsync(followerId, followingId, cancellationToken);
        await EnsureTargetIsNotPrivilegedAsync(followingId, "follow", cancellationToken);
        if (await IsBlockedAsync(followerId, followingId))
        {
            throw new BadRequestException("You cannot follow a user when either of you has blocked the other.");
        }

        var follow = await FindFollowAsync(followerId, followingId, cancellationToken);
        if (follow is null)
        {
            var isPrivate = await _context.Profiles
                .Where(profile => profile.UserId == followingId)
                .Select(profile => profile.IsPrivate)
                .SingleOrDefaultAsync(cancellationToken);
            follow = new Follow
            {
                FollowerId = followerId,
                FollowingId = followingId,
                Status = isPrivate ? FollowStatus.Pending : FollowStatus.Accepted
            };
            await _context.Follows.AddAsync(follow, cancellationToken);
            await SaveWithUniqueConflictAsync("A follow relationship or request already exists.", cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
        return await GetRelationshipResponseAsync(followerId, followingId, cancellationToken);
    }

    public async Task UnfollowAsync(long followerId, long followingId, CancellationToken cancellationToken)
    {
        ValidateDistinctUserIds(followerId, followingId);
        await using var transaction = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);
        await EnsureUsersExistAsync(followerId, followingId, cancellationToken);
        await EnsureNotBlockedAsync(followerId, followingId, cancellationToken);

        var follow = await FindFollowAsync(followerId, followingId, cancellationToken);
        if (follow is not null)
        {
            await RemoveFollowsAsync([follow], cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }

    public Task ApproveFollowRequestAsync(long recipientId, long requestId, CancellationToken cancellationToken) =>
        ChangeFollowRequestAsync(recipientId, requestId, approve: true, incoming: true, cancellationToken);

    public Task RejectFollowRequestAsync(
        long recipientId,
        long requestId,
        CancellationToken cancellationToken) =>
        ChangeFollowRequestAsync(recipientId, requestId, approve: false, incoming: true, cancellationToken);

    public Task CancelFollowRequestAsync(
        long followerId,
        long requestId,
        CancellationToken cancellationToken) =>
        ChangeFollowRequestAsync(followerId, requestId, approve: false, incoming: false, cancellationToken);

    public async Task<PagedResult<FollowRequestResponse>> GetFollowRequestsAsync(
        long userId,
        bool incoming,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        ValidatePage(page, pageSize);
        var requests = _context.Follows.Where(follow =>
            follow.Status == FollowStatus.Pending
            && (incoming ? follow.FollowingId == userId : follow.FollowerId == userId)
            && (incoming ? follow.Follower.Status : follow.Following.Status) != UserStatus.Blocked
            && !_context.Blocks.Any(block =>
                (block.BlockerId == userId
                    && block.BlockedId == (incoming ? follow.FollowerId : follow.FollowingId))
                || (block.BlockedId == userId
                    && block.BlockerId == (incoming ? follow.FollowerId : follow.FollowingId))));

        var totalCount = await requests.CountAsync(cancellationToken);
        var rows = await requests
            .OrderByDescending(follow => follow.CreatedAt)
            .ThenBy(follow => follow.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(follow => new
            {
                follow.Id,
                UserId = incoming ? follow.FollowerId : follow.FollowingId,
                Username = incoming ? follow.Follower.Username : follow.Following.Username,
                DisplayName = incoming
                    ? (follow.Follower.Profile!.IsPrivate
                        ? follow.Follower.Username
                        : follow.Follower.Profile.DisplayName)
                    : (follow.Following.Profile!.IsPrivate
                        ? follow.Following.Username
                        : follow.Following.Profile.DisplayName),
                ProfilePhoto = incoming
                    ? (follow.Follower.Profile!.IsPrivate ? null : follow.Follower.Profile.ProfilePhoto)
                    : (follow.Following.Profile!.IsPrivate ? null : follow.Following.Profile.ProfilePhoto),
                follow.CreatedAt
            })
            .ToListAsync(cancellationToken);
        var items = rows.Select(request => new FollowRequestResponse(
            request.Id,
            request.UserId,
            request.Username,
            request.DisplayName,
            request.ProfilePhoto,
            new DateTimeOffset(DateTime.SpecifyKind(request.CreatedAt, DateTimeKind.Utc))))
            .ToList();

        return PagedResult<FollowRequestResponse>.Create(items, page, pageSize, totalCount);
    }

    public Task<PagedResult<UserProfileResponse>> GetFollowersAsync(
        long viewerId,
        long userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken) =>
        GetRelatedProfilesAsync(viewerId, userId, following: false, page, pageSize, cancellationToken);

    public Task<PagedResult<UserProfileResponse>> GetFollowingAsync(
        long viewerId,
        long userId,
        int page,
        int pageSize,
        CancellationToken cancellationToken) =>
        GetRelatedProfilesAsync(viewerId, userId, following: true, page, pageSize, cancellationToken);

    public async Task<UserRelationshipResponse> GetRelationshipAsync(
        long viewerId,
        long userId,
        CancellationToken cancellationToken)
    {
        await EnsureProfileVisibleAsync(viewerId, userId, cancellationToken);
        return await GetRelationshipResponseAsync(viewerId, userId, cancellationToken);
    }

    public async Task RemoveRelationshipsAsync(
        long firstUserId,
        long secondUserId,
        CancellationToken cancellationToken)
    {
        var follows = await _context.Follows
            .Where(follow => (follow.FollowerId == firstUserId && follow.FollowingId == secondUserId)
                || (follow.FollowerId == secondUserId && follow.FollowingId == firstUserId))
            .ToListAsync(cancellationToken);
        await RemoveFollowsAsync(follows, cancellationToken);
    }

    private IQueryable<UserProfileResponse> ProfileQuery(
        long viewerId,
        IQueryable<Profile>? profiles = null)
    {
        profiles ??= _context.Profiles;
        return profiles.Select(profile => new UserProfileResponse(
            profile.UserId,
            profile.User.Username,
            profile.DisplayName,
            profile.Bio,
            profile.ProfilePhoto,
            profile.IsPrivate,
            _context.Follows.Count(follow =>
                follow.FollowingId == profile.UserId && follow.Status == FollowStatus.Accepted),
            _context.Follows.Count(follow =>
                follow.FollowerId == profile.UserId && follow.Status == FollowStatus.Accepted),
            _context.Follows.Any(follow =>
                follow.FollowerId == viewerId
                && follow.FollowingId == profile.UserId
                && follow.Status == FollowStatus.Accepted),
            _context.Follows.Any(follow =>
                follow.FollowerId == profile.UserId
                && follow.FollowingId == viewerId
                && follow.Status == FollowStatus.Accepted)));
    }

    private async Task<PagedResult<UserProfileResponse>> GetRelatedProfilesAsync(
        long viewerId,
        long userId,
        bool following,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        ValidatePage(page, pageSize);
        await EnsureProfileVisibleAsync(viewerId, userId, cancellationToken);

        IQueryable<Profile> profiles;
        if (following)
        {
            profiles = _context.Follows
                .Where(follow => follow.FollowerId == userId
                    && follow.Status == FollowStatus.Accepted)
                .Select(follow => follow.Following.Profile!);
        }
        else
        {
            profiles = _context.Follows
                .Where(follow => follow.FollowingId == userId
                    && follow.Status == FollowStatus.Accepted)
                .Select(follow => follow.Follower.Profile!);
        }

        profiles = profiles.Where(profile =>
            profile.User.Status != UserStatus.Blocked
            && (profile.UserId == viewerId
                || !profile.IsPrivate
                || _context.Follows.Any(viewerFollow => viewerFollow.FollowerId == viewerId
                    && viewerFollow.FollowingId == profile.UserId
                    && viewerFollow.Status == FollowStatus.Accepted))
            && !_context.Blocks.Any(block =>
                (block.BlockerId == viewerId && block.BlockedId == profile.UserId)
                || (block.BlockedId == viewerId && block.BlockerId == profile.UserId)));

        var totalCount = await profiles.CountAsync(cancellationToken);
        var pagedProfiles = profiles
            .OrderBy(profile => profile.User.Username)
            .ThenBy(profile => profile.UserId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
        var items = await ProfileQuery(viewerId, pagedProfiles)
            .ToListAsync(cancellationToken);
        return PagedResult<UserProfileResponse>.Create(items, page, pageSize, totalCount);
    }

    private async Task<UserRelationshipResponse> GetRelationshipResponseAsync(long viewerId, long userId, CancellationToken cancellationToken)
    {
        var followerCount = await _context.Follows.CountAsync(
            follow => follow.FollowingId == userId && follow.Status == FollowStatus.Accepted,
            cancellationToken);
        var followingCount = await _context.Follows.CountAsync(
            follow => follow.FollowerId == userId && follow.Status == FollowStatus.Accepted,
            cancellationToken);
        var isFollowing = await _context.Follows.AnyAsync(
            follow => follow.FollowerId == viewerId
                && follow.FollowingId == userId
                && follow.Status == FollowStatus.Accepted,
            cancellationToken);
        var isFollowedBy = await _context.Follows.AnyAsync(
            follow => follow.FollowerId == userId
                && follow.FollowingId == viewerId
                && follow.Status == FollowStatus.Accepted,
            cancellationToken);
        var hasPendingRequest = await _context.Follows.AnyAsync(
            follow => follow.FollowerId == viewerId
                && follow.FollowingId == userId
                && follow.Status == FollowStatus.Pending,
            cancellationToken);
        var isPrivate = await _context.Profiles
            .Where(profile => profile.UserId == userId)
            .Select(profile => profile.IsPrivate)
            .SingleOrDefaultAsync(cancellationToken);
        if (isPrivate && viewerId != userId && !isFollowing)
        {
            followerCount = 0;
            followingCount = 0;
        }

        return new UserRelationshipResponse(
            userId,
            isFollowing,
            isFollowedBy,
            hasPendingRequest,
            followerCount,
            followingCount);
    }

    private async Task EnsureUsersExistAsync(long firstUserId, long secondUserId, CancellationToken cancellationToken)
    {
        var users = await _context.Users
            .Where(user => user.Id == firstUserId || user.Id == secondUserId)
            .Select(user => new { user.Id, user.Status })
            .ToListAsync(cancellationToken);
        if (users.Count != 2 || users.Any(user => user.Status == UserStatus.Blocked))
        {
            throw new NotFoundException("The requested user could not be found.");
        }
    }

    private async Task EnsureTargetIsNotPrivilegedAsync(
        long userId,
        string operation,
        CancellationToken cancellationToken)
    {
        var role = await _context.Users
            .Where(user => user.Id == userId)
            .Select(user => user.Role.Name)
            .SingleAsync(cancellationToken);
        if (role is RoleName.Admin or RoleName.Moderator)
        {
            throw new BadRequestException($"You cannot {operation} an administrator or moderator.");
        }
    }

    private async Task EnsureProfileVisibleAsync(
        long viewerId,
        long userId,
        CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            throw new BadRequestException("User ID must be greater than zero.");
        }

        var profile = await _context.Profiles
            .Where(candidate => candidate.UserId == userId)
            .Select(candidate => new { candidate.User.Status, candidate.IsPrivate })
            .SingleOrDefaultAsync(cancellationToken);
        if (profile is null || profile.Status == UserStatus.Blocked)
        {
            throw new NotFoundException("The requested profile could not be found.");
        }

        await EnsureNotBlockedAsync(viewerId, userId, cancellationToken);
        if (viewerId != userId && profile.IsPrivate
            && !await IsFollowingAsync(viewerId, userId, cancellationToken))
        {
            throw new NotFoundException("The requested profile could not be found.");
        }
    }

    private async Task EnsureNotBlockedAsync(
        long firstUserId,
        long secondUserId,
        CancellationToken cancellationToken)
    {
        if (await IsBlockedAsync(firstUserId, secondUserId))
        {
            throw new NotFoundException("The requested user could not be found.");
        }
    }

    private Task<Follow?> FindFollowAsync(long followerId, long followingId, CancellationToken cancellationToken) =>
        _context.Follows.SingleOrDefaultAsync(
            follow => follow.FollowerId == followerId && follow.FollowingId == followingId, cancellationToken);

    private async Task ChangeFollowRequestAsync(long userId, long requestId, bool approve, bool incoming, CancellationToken cancellationToken)
    {
        if (requestId <= 0)
        {
            throw new BadRequestException("Follow request ID must be greater than zero.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable, cancellationToken);
        var follow = await _context.Follows.SingleOrDefaultAsync(candidate => candidate.Id == requestId
                && candidate.Status == FollowStatus.Pending
                // approve or reject => followingId other followerId
                && (incoming ? candidate.FollowingId == userId : candidate.FollowerId == userId),
            cancellationToken)
            ?? throw new NotFoundException("The pending follow request could not be found.");
        await EnsureNotBlockedAsync(follow.FollowerId, follow.FollowingId, cancellationToken);

        if (approve)
        {
            follow.Status = FollowStatus.Accepted;
        }
        else
        {
            await RemoveFollowsAsync([follow], cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    private async Task RemoveFollowsAsync(IReadOnlyCollection<Follow> follows, CancellationToken cancellationToken)
    {
        if (follows.Count == 0)
        {
            return;
        }

        var followIds = follows.Select(follow => follow.Id).ToArray();
        var notifications = await _context.Notifications
            .Where(notification => notification.FollowId.HasValue && followIds.Contains(notification.FollowId.Value))
            .ToListAsync(cancellationToken);
        foreach (var notification in notifications)
        {
            notification.FollowId = null;
        }

        _context.Follows.RemoveRange(follows);
    }

    private async Task SaveWithUniqueConflictAsync(string message, CancellationToken cancellationToken)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new ConflictException(message);
        }
    }

    private static void ValidateDistinctUserIds(long firstUserId, long secondUserId)
    {
        if (firstUserId <= 0 || secondUserId <= 0)
        {
            throw new BadRequestException("User IDs must be greater than zero.");
        }

        if (firstUserId == secondUserId)
        {
            throw new BadRequestException("You cannot perform this action on your own account.");
        }

    }

    private static void ValidatePage(int page, int pageSize)
    {
        if (page < 1 || pageSize < 1 || pageSize > 50 || (long)(page - 1) * pageSize > int.MaxValue)
        {
            throw new BadRequestException("Page must be at least 1 and page size must be between 1 and 50.");
        }
    }
}