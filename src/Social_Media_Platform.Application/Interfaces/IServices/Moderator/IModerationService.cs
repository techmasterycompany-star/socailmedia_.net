using Social_Media_Platform.Application.DTOS.Moderator;

namespace Social_Media_Platform.Application.Interfaces.IServices.Moderator
{
    public interface IModerationService
    {
        Task<ModerationActionResponse> HidePostAsync(long id, string? reason, long? actorId);
        Task<ModerationActionResponse> RemoveCommentAsync(long id, string? reason, long? actorId);
        Task<ModerationActionResponse> StopStreamAsync(long id ,string? reason, long? actorId);
    }
}
