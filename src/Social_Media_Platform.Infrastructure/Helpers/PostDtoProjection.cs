using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Domain.Entities;
using System.Linq.Expressions;

namespace Social_Media_Platform.Infrastructure.Helpers
{
    public static class PostDtoProjection
    {
        public static Expression<Func<Post, PostDto>> ForViewer(long viewerId) =>
            post => new PostDto(
                post.Id,
                post.AuthorId,
                post.Author.Username,
                post.Content,
                post.Visibility,
                post.CategoryId,
                post.CreatedAt,
                post.UpdatedAt,
                post.PostHashtags.Select(link => link.Hashtag.Name).ToList(),
                post.Media.OrderBy(media => media.DisplayOrder)
                    .Select(media => new PostMediaDto(
                        media.Id,
                        media.MediaType,
                        media.ContentType,
                        media.SizeBytes,
                        media.MediaUrl,
                        media.DisplayOrder
                    )).ToList(),
                post.Likes.Count(),
                post.Comments.Count(comment => !comment.IsDeleted),
                post.Likes.Any(like => like.UserId == viewerId),
                post.SavedBy.Any(saved => saved.UserId == viewerId)
            );
    }
}
