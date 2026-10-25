using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.DTOS.Post
{
    public sealed record PostMediaDto(long Id, MediaType MediaType, string ContentType, long SizeBytes, string Url, int DisplayOrder);
}
