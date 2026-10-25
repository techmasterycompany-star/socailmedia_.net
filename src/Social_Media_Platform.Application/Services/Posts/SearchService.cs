using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Application.Interfaces.IServices;

namespace Social_Media_Platform.Application.Services.Posts
{
    public sealed class SearchService(ISearchRepository searchRepository) : ISearchService
    {
        public async Task<PagedResult<PostDto>> SearchPostsAsync(
            string? query, string? hashtag, long viewerId, int page, int pageSize, CancellationToken cancellationToken)
        {
            (page, pageSize) = NormalizePagination(page, pageSize);

            if (string.IsNullOrWhiteSpace(query) && string.IsNullOrWhiteSpace(hashtag))
                return PagedResult<PostDto>.Create(Array.Empty<PostDto>(), page, pageSize, 0);

            var (posts, totalCount) = await searchRepository.SearchPostsAsync(query, hashtag, viewerId, page, pageSize, cancellationToken);

            return PagedResult<PostDto>.Create(posts, page, pageSize, totalCount);
        }

        public async Task<List<HashtagDto>> SearchHashtagsAsync(string? query, int limit, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(query))
                return [];

            var hashtags = await searchRepository.SearchHashtagsAsync(
                query.Trim().TrimStart('#'), Math.Clamp(limit, 1, 50), cancellationToken);

            return hashtags.Select(hashtag => new HashtagDto(hashtag.Id, hashtag.Name)).ToList();
        }

        public async Task<List<TrendingHashtagDto>> GetTrendingHashtagsAsync(int limit, CancellationToken cancellationToken)
        {
            var hashtags = await searchRepository.GetTrendingHashtagsAsync(Math.Clamp(limit, 1, 50), cancellationToken);
            return hashtags.ToList();
        }

        public async Task<PagedResult<UserSummaryDto>> SearchUsersAsync(
            string query, long viewerId, int page, int pageSize, CancellationToken cancellationToken)
        {
            (page, pageSize) = NormalizePagination(page, pageSize);

            if (string.IsNullOrWhiteSpace(query))
                return PagedResult<UserSummaryDto>.Create([], page, pageSize, 0);

            var (users, totalCount) = await searchRepository.SearchUsersAsync(query.Trim(), viewerId, page, pageSize, cancellationToken);
            var items = users.Select(user => new UserSummaryDto(user.Id, user.Username)).ToList();

            return PagedResult<UserSummaryDto>.Create(items, page, pageSize, totalCount);
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            var categories = await searchRepository.GetActiveCategoriesAsync(cancellationToken);
            return categories.Select(category => new CategoryDto(category.Id, category.Name)).ToList();
        }

        private static (int Page, int PageSize) NormalizePagination(int page, int pageSize)
        {
            pageSize = Math.Clamp(pageSize, 1, 100);
            page = Math.Clamp(page, 1, int.MaxValue / pageSize);
            return (page, pageSize);
        }
    }

}
