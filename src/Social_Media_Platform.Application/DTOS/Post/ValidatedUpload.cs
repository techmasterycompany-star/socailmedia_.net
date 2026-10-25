using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.DTOS.Post
{
    public sealed record ValidatedUpload(MediaType Kind, string ContentType, long SizeBytes, string Extension);
}
