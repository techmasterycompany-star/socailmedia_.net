using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Application.Interfaces.IRepositories
{
    public interface IPostInteractionRepository
    {
        Task<bool> PostExistsAsync(long postId, CancellationToken cancellationToken);
        Task<bool> IsPostLikedAsync(long postId, long userId, CancellationToken cancellationToken);
        Task<int> CountLikesAsync(long postId, CancellationToken cancellationToken);
        Task AddLikeAsync(PostLike like, CancellationToken cancellationToken);
        Task RemoveLikeAsync(long postId, long userId, CancellationToken cancellationToken);

        Task<(IReadOnlyList<Comment> Items, int TotalCount)> GetCommentsPageAsync(long postId, int page, int pageSize, CancellationToken ct);
        Task<Comment?> GetCommentByIdAsync(long commentId, CancellationToken cancellationToken);
        Task AddCommentAsync(Comment comment, CancellationToken cancellationToken);
        Task<bool> CommentExistsOnPostAsync(long commentId, long postId, CancellationToken cancellationToken);

        Task<bool> IsPostSavedAsync(long postId, long userId, CancellationToken cancellationToken);
        Task AddSavedPostAsync(SavedPost savedPost, CancellationToken cancellationToken);
        Task RemoveSavedPostAsync(long postId, long userId, CancellationToken cancellationToken);
        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
