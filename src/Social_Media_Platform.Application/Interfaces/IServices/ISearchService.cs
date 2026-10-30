using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOS.Post;

namespace Social_Media_Platform.Application.Interfaces.IServices
{
    public interface ISearchService
    {
        Task<PagedResult<PostDto>> SearchPostsAsync(string? query, string? hashtag, long viewerId, int page, int pageSize, CancellationToken cancellationToken);
        Task<List<HashtagDto>> SearchHashtagsAsync(string? query, int limit, CancellationToken cancellationToken);
        Task<List<TrendingHashtagDto>> GetTrendingHashtagsAsync(int limit, CancellationToken cancellationToken);
        Task<PagedResult<UserSummaryDto>> SearchUsersAsync(string query, long viewerId, int page, int pageSize, CancellationToken cancellationToken);
        Task<List<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken);
    }
}
