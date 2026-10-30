namespace Social_Media_Platform.Application.Interfaces.IServices;

public interface ILiveKitRoomService
{
    Task DeleteRoomAsync(string roomName);
}