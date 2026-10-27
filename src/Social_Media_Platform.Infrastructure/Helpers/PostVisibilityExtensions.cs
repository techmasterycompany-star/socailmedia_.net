using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.Infrastructure.Helpers
{
    public static class PostVisibilityExtensions
    {
        public static IQueryable<Post> VisibleTo(this IQueryable<Post> posts, Social_Media_PlatformDbContext dbContext, long viewerId)
        {
            var acceptedFollows = dbContext.Follows.Where(follow =>
                follow.FollowerId == viewerId && follow.Status == FollowStatus.Accepted);

            return posts
                .Where(post => !post.IsDeleted && !post.IsHidden && !post.Author.IsDeleted)
                .Where(post => post.AuthorId == viewerId ||
                    !dbContext.Blocks.Any(block =>
                        block.BlockerId == viewerId && block.BlockedId == post.AuthorId ||
                        block.BlockerId == post.AuthorId && block.BlockedId == viewerId) &&
                     (post.Author.Profile == null || !post.Author.Profile.IsPrivate ||
                      acceptedFollows.Any(follow => follow.FollowingId == post.AuthorId)) &&
                     (post.Visibility == PostVisibility.Public ||
                      post.Visibility == PostVisibility.FollowersOnly &&
                       acceptedFollows.Any(follow => follow.FollowingId == post.AuthorId)));
        }
    }
}
