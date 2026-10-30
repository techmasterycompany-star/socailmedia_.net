using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.DTOS.Post
{
    public sealed record PostDto(
        long Id,
        long AuthorId,
        string AuthorUsername,
        string? Text,
        PostVisibility Visibility,
        long? CategoryId,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        IReadOnlyList<string> Hashtags,
        IReadOnlyList<PostMediaDto> Media,
        int LikeCount,
        int CommentCount,
        bool LikedByMe,
        bool SavedByMe);
}
