using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Application.Interfaces.IServices.LiveStreaming;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Services.LiveStreaming;

public sealed class LiveStreamAccessService : ILiveStreamAccessService
{
    private readonly ILiveStreamRepository _liveStreamRepository;
    private readonly IFollowRepository _followRepository;

    public LiveStreamAccessService(ILiveStreamRepository liveStreamRepository, IFollowRepository followRepository)
    {
        _liveStreamRepository = liveStreamRepository;
        _followRepository = followRepository;
    }

    public async Task<LiveStream> ValidateCanJoinAsync(long userId, long streamId)
    {
        var stream = await _liveStreamRepository.GetByIdAsync(streamId);

        if (stream is null)
            throw new NotFoundException($"No live stream found with ID {streamId}.");

        if (stream.Status != LiveStreamStatus.Live)
            throw new InvalidOperationException("The live stream is not active.");

        if (userId != stream.HostId
            && await _followRepository.IsBlockedAsync(userId, stream.HostId))
        {
            throw new NotFoundException($"No live stream found with ID {streamId}.");
        }

        await ValidateFollowingAsync(userId, stream);

        return stream;
    }

    public async Task ValidateFollowingAsync(long userId, LiveStream stream)
    {
        if (stream.AudienceSetting == AudienceSetting.FollowersOnly && userId != stream.HostId)
        {
            var isFollowing = await _followRepository.IsFollowingAsync(userId, stream.HostId);

            if (!isFollowing)
                throw new UnauthorizedAccessException("You must follow the host to join this live stream.");
        }
    }

}