using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;
using Social_Media_Platform.Infrastructure.Data;
using Social_Media_Platform.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Social_Media_Platform.Infrastructure.Repositories.Posts
{
    public sealed class PostRepository(Social_Media_PlatformDbContext dbContext) : IPostRepository
    {
        public Task<Post?> GetByIdAsync(long postId, CancellationToken ct) =>
            GetPostQuery().AsSplitQuery().FirstOrDefaultAsync(post => post.Id == postId, ct);
        public Task<PostDto?> GetDtoByIdAsync(long postId, long viewerId, CancellationToken ct) =>
            GetAccessiblePostsQuery(viewerId)
                .Where(post => post.Id == postId)
                .Select(PostDtoProjection.ForViewer(viewerId))
                .FirstOrDefaultAsync(ct);
        public Task<Post?> GetByIdForUpdateAsync(long postId, CancellationToken ct) =>
            dbContext.Posts
                .Include(post => post.Author).ThenInclude(author => author.Profile)
                .Include(post => post.Media)
                .Include(post => post.PostHashtags).ThenInclude(link => link.Hashtag)
                .AsSplitQuery()
                .FirstOrDefaultAsync(post => post.Id == postId, ct);

        public Task<bool> IsAccessibleAsync(long postId, long viewerId, CancellationToken ct) =>
            GetAccessiblePostsQuery(viewerId).AnyAsync(post => post.Id == postId, ct);

        public Task AddAsync(Post post, CancellationToken ct) => dbContext.Posts.AddAsync(post, ct).AsTask();

        public Task SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);

        public Task<bool> IsBlockedEitherWayAsync(long firstUserId, long secondUserId, CancellationToken ct) =>
            dbContext.Blocks.AnyAsync(block =>
                (block.BlockerId == firstUserId && block.BlockedUserId == secondUserId) ||
                (block.BlockerId == secondUserId && block.BlockedUserId == firstUserId), ct);

        public Task<bool> IsAcceptedFollowerAsync(long followerId, long followingId, CancellationToken ct) =>
            dbContext.Follows.AnyAsync(follow =>
                follow.FollowerId == followerId &&
                follow.FollowingId == followingId &&
                follow.Status == FollowStatus.Accepted, ct);

        public Task<bool> CategoryExistsAsync(long categoryId, CancellationToken ct) =>
            dbContext.Categories.AnyAsync(category => category.Id == categoryId && category.IsActive, ct);

        public async Task<(IReadOnlyList<PostDto> Items, int TotalCount)> GetAccessibleSavedPageAsync(long userId, int page, int pageSize, CancellationToken ct)
        {
            var query = GetAccessiblePostsQuery(userId)
                .Where(post => dbContext.SavedPosts.Any(saved =>
                    saved.PostId == post.Id && saved.UserId == userId));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(post => post.CreatedAt).ThenByDescending(post => post.Id)
                .Skip((int)Math.Min(((long)page - 1) * pageSize, int.MaxValue)).Take(pageSize)
                .Select(PostDtoProjection.ForViewer(userId))
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<(IReadOnlyList<PostDto> Items, int TotalCount)> GetFeedPageAsync(long userId, int page, int pageSize, CancellationToken ct)
        {
            var query = GetBasePostQuery()
                .VisibleTo(dbContext, userId)
                .Where(post => post.AuthorId != userId)
                .Where(post => dbContext.Follows.Any(follow =>
                    follow.FollowerId == userId &&
                    follow.FollowingId == post.AuthorId &&
                    follow.Status == FollowStatus.Accepted));

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(post => post.CreatedAt).ThenByDescending(post => post.Id)
                .Skip((int)Math.Min(((long)page - 1) * pageSize, int.MaxValue)).Take(pageSize)
                .Select(PostDtoProjection.ForViewer(userId))
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<(IReadOnlyList<PostDto> Items, int TotalCount)> GetDiscoveryPageAsync(long userId, int page, int pageSize, CancellationToken ct)
        {
            var query = GetBasePostQuery()
                .VisibleTo(dbContext, userId)
                .Where(post => post.Visibility == PostVisibility.Public)
                .Where(post => post.Author.Profile == null || !post.Author.Profile.IsPrivate);

            var totalCount = await query.CountAsync(ct);
            var items = await query
                .OrderByDescending(post => post.CreatedAt).ThenByDescending(post => post.Id)
                .Skip((int)Math.Min(((long)page - 1) * pageSize, int.MaxValue)).Take(pageSize)
                .Select(PostDtoProjection.ForViewer(userId))
                .ToListAsync(ct);

            return (items, totalCount);
        }

        private IQueryable<Post> GetPostQuery() =>
            GetBasePostQuery()
                .Include(post => post.Author).ThenInclude(author => author.Profile)
                .Include(post => post.Media)
                .Include(post => post.PostHashtags).ThenInclude(link => link.Hashtag);

        private IQueryable<Post> GetBasePostQuery() =>
            dbContext.Posts.AsNoTracking()
                .Where(post => !post.IsDeleted && !post.IsHidden && !post.Author.IsDeleted);

        private IQueryable<Post> GetAccessiblePostsQuery(long userId) =>
            GetBasePostQuery().VisibleTo(dbContext, userId);

    }
}
