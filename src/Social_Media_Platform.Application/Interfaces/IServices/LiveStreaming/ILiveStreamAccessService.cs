using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Application.Interfaces.IServices.LiveStreaming;

public interface ILiveStreamAccessService
{
    Task<LiveStream> ValidateCanJoinAsync(long userId, long streamId);
    Task ValidateFollowingAsync(long userId, LiveStream stream);
}