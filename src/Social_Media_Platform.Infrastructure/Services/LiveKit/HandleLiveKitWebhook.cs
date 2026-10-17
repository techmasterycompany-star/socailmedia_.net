using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Infrastructure.Services.LiveKit;

public class HandleLiveKitWebhook
{
    private readonly ILiveStreamRepository _liveStreamRepository;

    public HandleLiveKitWebhook(ILiveStreamRepository liveStreamRepository)
    {
        _liveStreamRepository = liveStreamRepository;
    }

    public async Task HandleRoomStatusAsync(string eventName, string roomName)
    {

        if (string.IsNullOrWhiteSpace(roomName))
        {
            throw new BadRequestException("Room name cannot be null or empty.");
        }

        var liveStream = await _liveStreamRepository.GetByRoomNameAsync(roomName);

        if (liveStream == null)
        {
            throw new NotFoundException($"Live stream with room name '{roomName}' not found.");
        }

        switch (eventName)
        {
            case "room_started":
                if (liveStream.Status == LiveStreamStatus.Scheduled)
                {
                    liveStream.Status = LiveStreamStatus.Live;
                    liveStream.StartAt = DateTime.UtcNow;

                    await _liveStreamRepository.SaveChangesAsync();
                }
                break;

            case "room_finished":
                if (liveStream.Status == LiveStreamStatus.Live)
                {
                    liveStream.Status = LiveStreamStatus.Ended;
                    liveStream.EndedAt = DateTime.UtcNow;

                    await _liveStreamRepository.SaveChangesAsync();
                }
                break;

            default:
                throw new InvalidOperationException($"Unexpected event name: {eventName}");
        }

    }
}