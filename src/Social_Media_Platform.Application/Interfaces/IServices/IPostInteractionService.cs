using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOS.Post;

namespace Social_Media_Platform.Application.Interfaces.IServices
{
    public interface IPostInteractionService
    {
        Task<LikeStateDto> LikePostAsync(long postId, long userId, CancellationToken cancellationToken);
        Task<LikeStateDto> UnlikePostAsync(long postId, long userId, CancellationToken cancellationToken);
        Task<PagedResult<CommentDto>> GetCommentsAsync(long postId, long viewerId, int page, int pageSize, CancellationToken cancellationToken);
        Task<CommentDto> CreateCommentAsync(long postId, long userId, CreateCommentRequest request, CancellationToken cancellationToken);
        Task DeleteCommentAsync(long commentId, long userId, CancellationToken cancellationToken);
        Task<SaveStateDto> SavePostAsync(long postId, long userId, CancellationToken cancellationToken);
        Task<SaveStateDto> UnsavePostAsync(long postId, long userId, CancellationToken cancellationToken);
    }
}
