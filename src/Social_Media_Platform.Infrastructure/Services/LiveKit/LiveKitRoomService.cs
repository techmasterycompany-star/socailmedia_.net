using Livekit.Server.Sdk.Dotnet;
using Microsoft.Extensions.Options;
using Social_Media_Platform.Application.Interfaces.IServices;

namespace Social_Media_Platform.Infrastructure.Services.LiveKit;

public sealed class LiveKitRoomService : ILiveKitRoomService
{
    private readonly RoomServiceClient _roomServiceClient;

    public LiveKitRoomService(IOptions<LiveKitOptions> options)
    {
        var settings = options.Value;

        _roomServiceClient = new RoomServiceClient(
            settings.Url,
            settings.ApiKey,
            settings.ApiSecret);
    }

    public async Task DeleteRoomAsync(string roomName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(roomName);

        var request = new DeleteRoomRequest { Room = roomName };

        await _roomServiceClient.DeleteRoom(request);
    }
}