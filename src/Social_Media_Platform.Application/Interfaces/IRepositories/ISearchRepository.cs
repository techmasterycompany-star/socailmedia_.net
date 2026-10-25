using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Application.Interfaces.IRepositories
{
    public interface ISearchRepository
    {
        Task<(IReadOnlyList<PostDto> Items, int TotalCount)> SearchPostsAsync( string? query, string? hashtag, long viewerId, int page, int pageSize,CancellationToken cancellationToken);

        Task<IReadOnlyList<Hashtag>> SearchHashtagsAsync(string query, int limit, CancellationToken cancellationToken);
        Task<IReadOnlyList<TrendingHashtagDto>> GetTrendingHashtagsAsync(int limit, CancellationToken cancellationToken);

        Task<(IReadOnlyList<User> Items, int TotalCount)> SearchUsersAsync(string query, long viewerId, int page, int pageSize, CancellationToken ct);

        Task<IReadOnlyList<Category>> GetActiveCategoriesAsync(CancellationToken cancellationToken);
    }
}
