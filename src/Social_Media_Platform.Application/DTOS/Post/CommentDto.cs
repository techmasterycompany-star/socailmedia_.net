namespace Social_Media_Platform.Application.DTOS.Post
{
    public sealed record CommentDto(
        long Id,
        long PostId,
        long AuthorId,
        string Username,
        string Content,
        DateTime CreatedAt);
}
