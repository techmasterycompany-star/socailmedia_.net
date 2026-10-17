using Social_Media_Platform.Application.Interfaces.IServices;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Application.DTOS.LiveStream;
using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Services.LiveStreaming;

public class LiveStreamService : ILiveStreamService
{
    private readonly ILiveKitTokenService _liveKitTokenService;
    private readonly ILiveStreamRepository _liveStreamRepository;
    private readonly IFollowRepository _followRepository;
    private readonly ILiveKitRoomService _liveKitRoomService;


    public LiveStreamService(ILiveKitTokenService liveKitTokenService, ILiveStreamRepository liveStreamRepository, IFollowRepository followRepository, ILiveKitRoomService liveKitRoomService)
    {
        _liveKitTokenService = liveKitTokenService;
        _liveStreamRepository = liveStreamRepository;
        _followRepository = followRepository;
        _liveKitRoomService = liveKitRoomService;
    }

    public async Task<IEnumerable<LiveStreamDto>> GetAllAsync()
    {
        var liveStreams = await _liveStreamRepository.GetAllAsync();

        return liveStreams.Select(ls => new LiveStreamDto
        {
            Id = ls.Id,
            HostId = ls.HostId,
            Title = ls.Title,
            Status = ls.Status,
            AudienceSetting = ls.AudienceSetting,
            CreatedAt = ls.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        });
    }
    public async Task<StartLiveStreamResponse> ExecuteStartAsync(long hostId, StartLiveStreamRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (hostId <= 0)
        {
            throw new ArgumentException("Host ID must be a positive number.", nameof(hostId));
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Title cannot be null or empty.");
        }

        if (await _liveStreamRepository.HasActiveStreamAsync(hostId))
        {
            throw new InvalidOperationException("The user already has an active live stream.");
        }

        var roomName = $"stream-{Guid.NewGuid():N}";

        LiveStream newLiveStream = new LiveStream
        {
            HostId = hostId,
            Title = request.Title,
            AudienceSetting = request.AudienceSetting,
            RoomName = roomName,
        };


        var token = _liveKitTokenService.GenerateToken(hostId.ToString(), roomName, true);


        await _liveStreamRepository.AddLiveStreamAsync(newLiveStream);
        await _liveStreamRepository.SaveChangesAsync();

        var response = new StartLiveStreamResponse
        {
            StreamId = newLiveStream.Id,
            RoomName = roomName,
            ServerUrl = _liveKitTokenService.ServerUrl,
            Token = token,
        };

        return response;
    }

    public async Task<StartLiveStreamResponse> ExecuteJoinAsync(long viewerId, long streamId)
    {
        var liveStream = await _liveStreamRepository.GetByIdAsync(streamId);

        if (liveStream == null)
        {
            throw new NotFoundException($"No live stream found for stream with ID {streamId}.");
        }

        if (liveStream.Status != LiveStreamStatus.Live)
        {
            throw new InvalidOperationException("The live stream is not active.");
        }

        if (liveStream.AudienceSetting == AudienceSetting.FollowersOnly)
        {
            var isFollowing = await _followRepository.IsFollowingAsync(
                    viewerId,
                    liveStream.HostId);

            if (!isFollowing)
            {
                throw new UnauthorizedAccessException("You must follow the host to join this live stream.");
            }
        }

        var token = _liveKitTokenService.GenerateToken(viewerId.ToString(), liveStream.RoomName, false);

        return new StartLiveStreamResponse
        {
            StreamId = liveStream.Id,
            RoomName = liveStream.RoomName,
            ServerUrl = _liveKitTokenService.ServerUrl,
            Token = token
        };
    }

    public async Task ExecuteEndAsync(long hostId, long streamId)
    {
        if (hostId <= 0)
            throw new ArgumentOutOfRangeException();

        if (streamId <= 0)
            throw new ArgumentOutOfRangeException();

        var liveStream =
            await _liveStreamRepository.GetByIdAsync(streamId);

        if (liveStream is null)
        {
            throw new NotFoundException($"Live stream with ID {streamId} was not found.");
        }

        if (liveStream.HostId != hostId)
        {
            throw new UnauthorizedAccessException("You are not the host of this live stream.");
        }

        if (liveStream.Status != LiveStreamStatus.Live)
        {
            throw new InvalidOperationException("Only an active live stream can be ended.");
        }

        await _liveKitRoomService.DeleteRoomAsync(liveStream.RoomName);
    }

}