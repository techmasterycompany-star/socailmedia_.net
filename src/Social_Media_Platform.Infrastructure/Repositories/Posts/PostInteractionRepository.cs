using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.Infrastructure.Repositories.Posts
{
    public sealed class PostInteractionRepository(Social_Media_PlatformDbContext dbContext) : IPostInteractionRepository
    {
        public Task<bool> PostExistsAsync(long postId, CancellationToken ct) =>
            dbContext.Posts.AnyAsync(post => post.Id == postId && !post.IsDeleted && !post.IsHidden && !post.Author.IsDeleted, ct);

        public Task<bool> IsPostLikedAsync(long postId, long userId, CancellationToken ct) =>
            dbContext.PostLikes.AnyAsync(like => like.PostId == postId && like.UserId == userId, ct);

        public Task<int> CountLikesAsync(long postId, CancellationToken ct) =>
            dbContext.PostLikes.CountAsync(like => like.PostId == postId, ct);

        public Task AddLikeAsync(PostLike like, CancellationToken ct) =>
            dbContext.PostLikes.AddAsync(like, ct).AsTask();

        public async Task RemoveLikeAsync(long postId, long userId, CancellationToken ct)
        {
            var like = await dbContext.PostLikes.FirstOrDefaultAsync(
                item => item.PostId == postId && item.UserId == userId, ct);

            if (like is not null)
                dbContext.PostLikes.Remove(like);
        }

        public async Task<(IReadOnlyList<Comment> Items, int TotalCount)> GetCommentsPageAsync(long postId, int page, int pageSize, CancellationToken ct)
        {
            var comments = dbContext.Comments
                .AsNoTracking()
                .Where(comment => comment.PostId == postId && !comment.IsDeleted);

            var totalCount = await comments.CountAsync(ct);

            var items = await comments
                .Include(comment => comment.Author)
                .OrderByDescending(comment => comment.CreatedAt)
                .ThenByDescending(comment => comment.Id)
                .Skip((int)Math.Min(((long)page - 1) * pageSize, int.MaxValue))
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public Task<Comment?> GetCommentByIdAsync(long commentId, CancellationToken ct) =>
            dbContext.Comments
                .Include(comment => comment.Author)
                .Include(comment => comment.Post)
                .FirstOrDefaultAsync(comment => comment.Id == commentId && !comment.IsDeleted, ct);

        public Task AddCommentAsync(Comment comment, CancellationToken ct) =>
            dbContext.Comments.AddAsync(comment, ct).AsTask();

        public Task<bool> CommentExistsOnPostAsync(long commentId, long postId, CancellationToken ct) =>
            dbContext.Comments.AnyAsync(comment =>
                comment.Id == commentId && comment.PostId == postId && !comment.IsDeleted, ct);

        public Task<bool> IsPostSavedAsync(long postId, long userId, CancellationToken ct) =>
            dbContext.SavedPosts.AnyAsync(saved => saved.PostId == postId && saved.UserId == userId, ct);

        public Task AddSavedPostAsync(SavedPost savedPost, CancellationToken ct) =>
            dbContext.SavedPosts.AddAsync(savedPost, ct).AsTask();

        public async Task RemoveSavedPostAsync(long postId, long userId, CancellationToken ct)
        {
            var saved = await dbContext.SavedPosts.FirstOrDefaultAsync(
                item => item.PostId == postId && item.UserId == userId, ct);

            if (saved is not null)
                dbContext.SavedPosts.Remove(saved);
        }

        public Task SaveChangesAsync(CancellationToken ct) => dbContext.SaveChangesAsync(ct);
    }
}
