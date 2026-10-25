using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;
using Social_Media_Platform.Infrastructure.Data;
using Social_Media_Platform.Infrastructure.Helpers;

namespace Social_Media_Platform.Infrastructure.Repositories.Posts
{
    public sealed class SearchRepository(Social_Media_PlatformDbContext dbContext) : ISearchRepository
    {
        public async Task<(IReadOnlyList<PostDto> Items, int TotalCount)> SearchPostsAsync(
            string? query, string? hashtag, long viewerId, int page, int pageSize, CancellationToken ct)
        {
            var posts = GetAccessiblePostsQuery(viewerId);

            if (!string.IsNullOrWhiteSpace(hashtag))
            {
                var normalizedHashtag = HashtagNormalizer.Normalize(hashtag);
                posts = posts.Where(post => post.PostHashtags.Any(link =>
                    link.Hashtag.NormalizedName == normalizedHashtag));
            }

            if (!string.IsNullOrWhiteSpace(query))
            {
                var normalizedQuery = query.Trim().ToLowerInvariant();
                posts = posts.Where(post => post.Content != null &&
                    post.Content.ToLower().Contains(normalizedQuery));
            }

            if (string.IsNullOrWhiteSpace(query) && string.IsNullOrWhiteSpace(hashtag))
                return (Array.Empty<PostDto>(), 0);

            var totalCount = await posts.CountAsync(ct);

            var items = await posts
                .OrderByDescending(post => post.CreatedAt)
                .ThenByDescending(post => post.Id)
                .Skip((int)Math.Min(((long)page - 1) * pageSize, int.MaxValue))
                .Take(pageSize)
                .Select(PostDtoProjection.ForViewer(viewerId))
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<IReadOnlyList<Hashtag>> SearchHashtagsAsync(string query, int limit, CancellationToken ct)
        {
            var normalizedQuery = HashtagNormalizer.Normalize(query);
            if (normalizedQuery.Length == 0)
                return [];

            return await dbContext.Hashtags
                .AsNoTracking()
                .Where(hashtag => hashtag.NormalizedName.Contains(normalizedQuery))
                .OrderBy(hashtag => hashtag.NormalizedName).ThenBy(hashtag => hashtag.Id)
                .Take(limit)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<TrendingHashtagDto>> GetTrendingHashtagsAsync(int limit, CancellationToken ct)
        {
            var since = DateTime.UtcNow.AddDays(-7);

            return await dbContext.PostHashtags
                .AsNoTracking()
                .Where(link => link.Post.CreatedAt >= since &&
                               !link.Post.IsDeleted &&
                               !link.Post.IsHidden &&
                               !link.Post.Author.IsDeleted &&
                               link.Post.Visibility == PostVisibility.Public &&
                               (link.Post.Author.Profile == null || !link.Post.Author.Profile.IsPrivate))
                .GroupBy(link => new { link.Hashtag.Id, link.Hashtag.Name })
                .Select(group => new
                {
                    group.Key.Id,
                    group.Key.Name,
                    PostCount = group.Select(link => link.PostId).Distinct().Count()
                })
                .OrderByDescending(item => item.PostCount).ThenBy(item => item.Name)
                .Take(limit)
                .Select(item => new TrendingHashtagDto(item.Id, item.Name, item.PostCount))
                .ToListAsync(ct);
        }

        public async Task<(IReadOnlyList<User> Items, int TotalCount)> SearchUsersAsync(string query, long viewerId, int page, int pageSize, CancellationToken ct)
        {
            var usersQuery = GetSearchUsersQuery(query, viewerId);

            var totalCount = await usersQuery.CountAsync(ct);

            var items = await usersQuery
                .OrderBy(user => user.Username)
                .ThenBy(user => user.Id)
                .Skip((int)Math.Min(((long)page - 1) * pageSize, int.MaxValue))
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<IReadOnlyList<Category>> GetActiveCategoriesAsync(CancellationToken ct) =>
            await dbContext.Categories
                .AsNoTracking()
                .Where(category => category.IsActive)
                .OrderBy(category => category.Name).ThenBy(category => category.Id)
                .ToListAsync(ct);

        private IQueryable<Post> GetAccessiblePostsQuery(long viewerId) =>
            dbContext.Posts.AsNoTracking().VisibleTo(dbContext, viewerId);

        private IQueryable<User> GetSearchUsersQuery(string query, long viewerId)
        {
            var normalizedQuery = query.Trim().ToLowerInvariant();

            return dbContext.Users
                .AsNoTracking()
                .Where(user => user.Id != viewerId && !user.IsDeleted &&
                    user.Username.ToLower().Contains(normalizedQuery))
                .Where(user => !dbContext.Blocks.Any(block =>
                    (block.BlockerId == viewerId && block.BlockedUserId == user.Id) ||
                    (block.BlockerId == user.Id && block.BlockedUserId == viewerId)));
        }
    }
}
