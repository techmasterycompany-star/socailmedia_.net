using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.Common.DTOs.UserManagement;
using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.Infrastructure.Repositories;

public sealed class UserManagementRepository : IUserManagementRepository
{
    private readonly Social_Media_PlatformDbContext _context;
    private readonly IFollowRepository _followRepository;

    public UserManagementRepository(
        Social_Media_PlatformDbContext context,
        IFollowRepository followRepository)
    {
        _context = context;
        _followRepository = followRepository;
    }
    public async Task<UserProfileResponse> UpdateProfileAsync(long userId, UpdateUserProfileRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .Include(candidate => candidate.Profile)
            .SingleOrDefaultAsync(candidate => candidate.Id == userId, cancellationToken)
            ?? throw new NotFoundException("The requested user could not be found.");

        var profile = user.Profile;
        if (profile is null)
        {
            profile = new Profile { UserId = user.Id, User = user };
            user.Profile = profile;
            await _context.Profiles.AddAsync(profile, cancellationToken);
        }

        if (request.Username is not null)
        {
            if (await _context.Users.AnyAsync(
                    candidate => candidate.Id != userId && candidate.Username == request.Username,
                    cancellationToken))
            {
                throw new ConflictException("That username is already in use.");
            }

            user.Username = request.Username;
        }

        if (request.DisplayName is not null)
        {

            profile.DisplayName = request.DisplayName;
        }

        if (request.Bio is not null)
        {
            profile.Bio = request.Bio;
        }

        if (request.ProfilePhoto is not null)
        {
            if (request.ProfilePhoto.Length > 2048
                || (request.ProfilePhoto.Length > 0
                    && (!Uri.TryCreate(request.ProfilePhoto, UriKind.Absolute, out var photoUri)
                        || (photoUri.Scheme != Uri.UriSchemeHttp && photoUri.Scheme != Uri.UriSchemeHttps))))
            {
                throw new BadRequestException("Profile picture must be an HTTP or HTTPS URL no longer than 2048 characters.");
            }

            profile.ProfilePhoto = request.ProfilePhoto.Length == 0 ? null : request.ProfilePhoto;
        }

        if (request.IsPrivate.HasValue)
        {
            profile.IsPrivate = request.IsPrivate.Value;
        }

        await _context.SaveChangesAsync(cancellationToken);

        return await _followRepository.GetVisibleProfileAsync(userId, userId, cancellationToken);
    }

    public async Task BlockAsync(long blockerId, long blockedId, CancellationToken cancellationToken)
    {
        ValidateDistinctUserIds(blockerId, blockedId);
        await using var transaction = await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);
        await EnsureUsersExistAsync(blockerId, blockedId, cancellationToken);

        await _followRepository.RemoveRelationshipsAsync(blockerId, blockedId, cancellationToken);

        var block = await _context.Blocks
            .SingleOrDefaultAsync(candidate =>
                candidate.BlockerId == blockerId && candidate.BlockedId == blockedId,
                cancellationToken);
        if (block is null)
        {
            await _context.Blocks.AddAsync(
                new Block { BlockerId = blockerId, BlockedId = blockedId },
                cancellationToken);
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            throw new ConflictException("The block already exists.");
        }

        await transaction.CommitAsync(cancellationToken);
    }

    public async Task UnblockAsync(long blockerId, long blockedId, CancellationToken cancellationToken)
    {
        ValidateDistinctUserIds(blockerId, blockedId);
        await EnsureUsersExistAsync(blockerId, blockedId, cancellationToken);
        var block = await _context.Blocks.SingleOrDefaultAsync(
            candidate => candidate.BlockerId == blockerId && candidate.BlockedId == blockedId,
            cancellationToken);
        if (block is null)
        {
            return;
        }

        _context.Blocks.Remove(block);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task EnsureUsersExistAsync(
        long firstUserId,
        long secondUserId,
        CancellationToken cancellationToken)
    {
        var existingUsers = await _context.Users
            .Where(user => user.Id == firstUserId || user.Id == secondUserId)
            .Select(user => new { user.Id, user.Status })
            .ToListAsync(cancellationToken);
        if (existingUsers.Count != 2 || existingUsers.Any(user => user.Status == UserStatus.Blocked))
        {
            throw new NotFoundException("The requested user could not be found.");
        }
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: 2601 or 2627 };

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

}
