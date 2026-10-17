using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Application.Interfaces.IRepositories.Moderator
{
    public interface IModerationRepository
    {
        Task<Post?> GetPostAsync(long id);
        Task<Comment?> GetCommentAsync(long id);
        Task<LiveStream?> GetStreamAsync(long id);
        Task UpdatePostAsync(Post post);
        Task DeleteCommentAsync(Comment comment);
        Task UpdateStreamAsync(LiveStream stream);
        Task AddLogAsync(ModerationLog log);
    }
}
