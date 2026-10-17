namespace Social_Media_Platform.Application.Interfaces.IServices;

public interface ILiveKitTokenService
{
    string ServerUrl { get; }
    string GenerateToken(string identity, string roomName, bool canPublish);
}