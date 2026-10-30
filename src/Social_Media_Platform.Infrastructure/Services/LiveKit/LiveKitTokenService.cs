using Livekit.Server.Sdk.Dotnet;
using Microsoft.Extensions.Options;
using Social_Media_Platform.Application.Interfaces.IServices;

namespace Social_Media_Platform.Infrastructure.Services.LiveKit;

public class LiveKitTokenService : ILiveKitTokenService
{
    private readonly LiveKitOptions _options;

    public LiveKitTokenService(IOptions<LiveKitOptions> options)
    {
        _options = options.Value;
    }

    public string ServerUrl => _options.Url;
    public string GenerateToken(string identity, string roomName, bool canPublish)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(identity);
        ArgumentException.ThrowIfNullOrWhiteSpace(roomName);

        // define the user permissions inside the livekit room
        var grants = new VideoGrants
        {
            RoomJoin = true,
            Room = roomName,
            CanPublish = canPublish,
            CanSubscribe = true
        };

        var token = new AccessToken(
                _options.ApiKey,
                _options.ApiSecret)
            .WithIdentity(identity)
            .WithGrants(grants)
            .WithTtl(TimeSpan.FromMinutes(15));

        return token.ToJwt();
    }
}
