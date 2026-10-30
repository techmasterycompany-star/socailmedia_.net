using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Application.Interfaces.IRepositories
{
    public interface IHashtagRepository
    {
        Task AttachHashtagsAsync(Post post, string? text, CancellationToken cancellationToken);
    }
}
