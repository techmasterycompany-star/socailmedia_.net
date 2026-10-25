using Social_Media_Platform.Application.Interfaces.IServices;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Application.DTOS.LiveStream;
using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Domain.Enums;
using Social_Media_Platform.Application.DTOS.LiveStreaming;
using Social_Media_Platform.Application.Interfaces.IServices.LiveStreaming;

namespace Social_Media_Platform.Application.Services.LiveStreaming;

public class LiveStreamService : ILiveStreamService
{
    private readonly ILiveKitTokenService _liveKitTokenService;
    private readonly ILiveStreamRepository _liveStreamRepository;
    private readonly ILiveKitRoomService _liveKitRoomService;
    private readonly ILiveStreamAccessService _liveStreamAccessService;
    private const int MaximumPageSize = 50;

    public LiveStreamService(ILiveKitTokenService liveKitTokenService, ILiveStreamRepository liveStreamRepository, ILiveKitRoomService liveKitRoomService, ILiveStreamAccessService liveStreamAccessService)
    {
        _liveKitTokenService = liveKitTokenService;
        _liveStreamRepository = liveStreamRepository;
        _liveKitRoomService = liveKitRoomService;
        _liveStreamAccessService = liveStreamAccessService;
    }

    public async Task<PagedResult<LiveStreamDto>> GetAllAsync(int page, int pageSize)
    {
        ValidatePagination(page, pageSize);
        var (items, totalCount) = await _liveStreamRepository.GetAllAsync(page, pageSize);

        var liveStreams = items.Select(ls => new LiveStreamDto
        {
            Id = ls.Id,
            HostId = ls.HostId,
            Title = ls.Title,
            Status = ls.Status,
            AudienceSetting = ls.AudienceSetting,
            CreatedAt = ls.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        }).ToList();

        return PagedResult<LiveStreamDto>.Create(liveStreams, page, pageSize, totalCount);
    }

    public async Task<LiveStreamDto> ExecuteGetByIdAsync(long streamId)
    {
        if (streamId <= 0)
            throw new ArgumentOutOfRangeException(nameof(streamId), "Stream ID must be a positive number.");

        var liveStream = await _liveStreamRepository.GetByIdAsync(streamId);
        if (liveStream is null)
            throw new NotFoundException($"Live stream with ID {streamId} was not found.");

        return new LiveStreamDto
        {
            Id = liveStream.Id,
            HostId = liveStream.HostId,
            Title = liveStream.Title,
            Status = liveStream.Status,
            AudienceSetting = liveStream.AudienceSetting,
            CreatedAt = liveStream.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
        };
    }

    public async Task<StartLiveStreamResponse> ExecuteStartAsync(long hostId, StartLiveStreamRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateStartParams(hostId, request);

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
        var liveStream = await _liveStreamAccessService.ValidateCanJoinAsync(viewerId, streamId);

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
        if (hostId <= 0 || streamId <= 0)
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

    public async Task<StreamChatMessageDTO> ExecuteSendMessageAsync(long senderId, long streamId, string content, StreamMessageType messageType)
    {
        ValidateSendMessageParams(senderId, streamId, content);

        var liveStream = await _liveStreamAccessService.ValidateCanJoinAsync(senderId, streamId);

        var message = new StreamMessage
        {
            StreamId = streamId,
            SenderId = senderId,
            Content = content,
            MessageType = messageType,
            CreatedAt = DateTime.UtcNow
        };

        await _liveStreamRepository.AddMessageAsync(message);
        await _liveStreamRepository.SaveChangesAsync();

        var response = new StreamChatMessageDTO
        {
            Id = message.Id,
            StreamId = message.StreamId,
            SenderId = message.SenderId,
            Content = message.Content,
            CreatedAt = message.CreatedAt
        };

        return response;
    }

    public async Task<PagedResult<StreamChatMessageDTO>> ExecuteGetMessagesAsync(long viewerId, long streamId, int page, int pageSize)
    {
        if (streamId <= 0)
            throw new ArgumentOutOfRangeException(nameof(streamId), "Stream ID must be a positive number.");

        ValidatePagination(page, pageSize);

        var liveStream = await _liveStreamRepository.GetByIdAsync(streamId);
        if (liveStream is null)
            throw new NotFoundException($"Live stream with ID {streamId} was not found.");


        await _liveStreamAccessService.ValidateFollowingAsync(viewerId, liveStream);

        var (items, totalCount) = await _liveStreamRepository.GetMessagesByStreamIdAsync(streamId, page, pageSize);

        var messages = items.Select(m => new StreamChatMessageDTO
        {
            Id = m.Id,
            StreamId = m.StreamId,
            SenderId = m.SenderId,
            Content = m.Content,
            CreatedAt = m.CreatedAt
        }).ToList();

        return PagedResult<StreamChatMessageDTO>.Create(messages, page, pageSize, totalCount);
    }

    private static void ValidatePagination(int page, int pageSize)
    {
        if (page <= 0)
            throw new ArgumentOutOfRangeException("Page must be a positive number.");

        if (pageSize <= 0 || pageSize > MaximumPageSize)
            throw new ArgumentOutOfRangeException($"Page size must be between 1 and {MaximumPageSize}.");
    }
    private static void ValidateSendMessageParams(long senderId, long streamId, string content)
    {
        if (senderId <= 0)
            throw new ArgumentOutOfRangeException("Sender ID must be a positive number.");

        if (streamId <= 0)
            throw new ArgumentOutOfRangeException("Stream ID must be a positive number.");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Message content cannot be null or empty.");
    }
    private static void ValidateStartParams(long hostId, StartLiveStreamRequest request)
    {
        if (hostId <= 0)
        {
            throw new ArgumentException("Host ID must be a positive number.", nameof(hostId));
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            throw new ArgumentException("Title cannot be null or empty.");
        }
    }
}