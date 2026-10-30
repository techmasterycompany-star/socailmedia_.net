using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Application.Interfaces.IRepositories
{
    public interface IPostRepository
    {
        Task<Post?> GetByIdAsync(long postId, CancellationToken cancellationToken);

        Task<Post?> GetByIdForUpdateAsync(long postId, CancellationToken cancellationToken);
        Task<PostDto?> GetDtoByIdAsync(long postId, long viewerId, CancellationToken cancellationToken);

        Task AddAsync(Post post, CancellationToken cancellationToken);
        Task<bool> IsBlockedEitherWayAsync(long firstUserId, long secondUserId, CancellationToken cancellationToken);
        Task<bool> IsAcceptedFollowerAsync(long followerId, long followingId, CancellationToken cancellationToken);
        Task<bool> CategoryExistsAsync(long categoryId, CancellationToken cancellationToken);
        Task<bool> IsAccessibleAsync(long postId, long viewerId, CancellationToken cancellationToken);

        Task<(IReadOnlyList<PostDto> Items, int TotalCount)> GetAccessibleSavedPageAsync(long userId, int page, int pageSize, CancellationToken ct);
        Task<(IReadOnlyList<PostDto> Items, int TotalCount)> GetFeedPageAsync(long userId, int page, int pageSize, CancellationToken ct);
        Task<(IReadOnlyList<PostDto> Items, int TotalCount)> GetDiscoveryPageAsync(long userId, int page, int pageSize, CancellationToken ct);
        Task<(IReadOnlyList<PostDto> Items, int TotalCount)> GetAuthorPostsPageAsync(long authorId, long viewerId, int page, int pageSize, CancellationToken ct);

        Task SaveChangesAsync(CancellationToken cancellationToken);
    }
}
