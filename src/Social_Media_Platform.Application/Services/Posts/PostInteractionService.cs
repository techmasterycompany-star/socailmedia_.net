using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Application.Interfaces.IServices;
using Social_Media_Platform.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Social_Media_Platform.Application.Services.Posts
{
    public sealed class PostInteractionService(IPostInteractionRepository interactionRepository, IPostRepository postRepository) : IPostInteractionService
    {
        public async Task<LikeStateDto> LikePostAsync(long postId, long userId, CancellationToken cancellationToken)
        {
            await EnsurePostAccessibleAsync(postId, userId, cancellationToken);

            if (!await interactionRepository.IsPostLikedAsync(postId, userId, cancellationToken))
            {
                await interactionRepository.AddLikeAsync(new PostLike { PostId = postId, UserId = userId }, cancellationToken);
                await interactionRepository.SaveChangesAsync(cancellationToken);
            }

            return new LikeStateDto(true, await interactionRepository.CountLikesAsync(postId, cancellationToken));
        }

        public async Task<LikeStateDto> UnlikePostAsync(long postId, long currentUserId, CancellationToken cancellationToken)
        {
            await EnsurePostAccessibleAsync(postId, currentUserId, cancellationToken);

            if (await interactionRepository.IsPostLikedAsync(postId, currentUserId, cancellationToken))
            {
                await interactionRepository.RemoveLikeAsync(postId, currentUserId, cancellationToken);
                await interactionRepository.SaveChangesAsync(cancellationToken);
            }

            var likeCount = await interactionRepository.CountLikesAsync(postId, cancellationToken);
            return new LikeStateDto(false, likeCount);
        }

        public async Task<PagedResult<CommentDto>> GetCommentsAsync(long postId, long viewerId, int page, int pageSize, CancellationToken cancellationToken)
        {
            await EnsurePostAccessibleAsync(postId, viewerId, cancellationToken);
            (page, pageSize) = NormalizePagination(page, pageSize);

            var (comments, totalCount) = await interactionRepository.GetCommentsPageAsync(postId, page, pageSize, cancellationToken);

            return PagedResult<CommentDto>.Create(comments.Select(MapComment).ToList(), page, pageSize, totalCount);
        }

        public async Task<CommentDto> CreateCommentAsync(long postId, long userId, CreateCommentRequest request, CancellationToken cancellationToken)
        {
            await EnsurePostAccessibleAsync(postId, userId, cancellationToken);

            var content = request.Content?.Trim();
            if (string.IsNullOrWhiteSpace(content))
                throw new ValidationException("Comment content cannot be empty.");

            if (content.Length > 2000)
                throw new ValidationException("Comment content cannot exceed 2000 characters.");

            var now = DateTime.UtcNow;
            var comment = new Comment
            {
                PostId = postId,
                AuthorId = userId,
                Content = content,
                CreatedAt = now,
                UpdatedAt = now
            };

            await interactionRepository.AddCommentAsync(comment, cancellationToken);
            await interactionRepository.SaveChangesAsync(cancellationToken);

            var createdComment = await interactionRepository.GetCommentByIdAsync(comment.Id, cancellationToken)
                ?? throw new InvalidOperationException("The comment could not be retrieved after creation.");

            return MapComment(createdComment);
        }

        public async Task DeleteCommentAsync(long commentId, long userId, CancellationToken cancellationToken)
        {
            var comment = await interactionRepository.GetCommentByIdAsync(commentId, cancellationToken)
                ?? throw new NotFoundException("Comment not found.");

            if (comment.AuthorId != userId && comment.Post.AuthorId != userId)
                throw new UnauthorizedAccessException("You can only delete your own comments or comments on your posts.");

            comment.IsDeleted = true;
            comment.DeletedAt = DateTime.UtcNow;
            comment.UpdatedAt = DateTime.UtcNow;

            await interactionRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<SaveStateDto> SavePostAsync(long postId, long userId, CancellationToken cancellationToken)
        {
            await EnsurePostAccessibleAsync(postId, userId, cancellationToken);

            if (!await interactionRepository.IsPostSavedAsync(postId, userId, cancellationToken))
            {
                await interactionRepository.AddSavedPostAsync(new SavedPost { PostId = postId, UserId = userId }, cancellationToken);
                await interactionRepository.SaveChangesAsync(cancellationToken);
            }

            return new SaveStateDto(true);
        }

        public async Task<SaveStateDto> UnsavePostAsync(long postId, long userId, CancellationToken cancellationToken)
        {
            var post = await postRepository.GetByIdAsync(postId, cancellationToken) ?? throw new NotFoundException("Post not found.");
            await interactionRepository.RemoveSavedPostAsync(postId, userId, cancellationToken);
            await interactionRepository.SaveChangesAsync(cancellationToken);

            return new SaveStateDto(false);
        }

        private async Task EnsurePostAccessibleAsync(long postId, long viewerId, CancellationToken cancellationToken)
        {
            if (!await postRepository.IsAccessibleAsync(postId, viewerId, cancellationToken))
                throw new NotFoundException("Post not found.");
        }

        private static CommentDto MapComment(Comment comment) =>
            new(comment.Id, comment.PostId, comment.AuthorId, comment.Author?.Username ?? string.Empty, comment.Content, comment.CreatedAt);

        private static (int Page, int PageSize) NormalizePagination(int page, int pageSize)
        {
            pageSize = Math.Clamp(pageSize, 1, 100);
            page = Math.Clamp(page, 1, int.MaxValue / pageSize);
            return (page, pageSize);
        }
    }

}
