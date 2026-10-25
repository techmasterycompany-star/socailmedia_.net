using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Application.Interfaces.IServices
{
    public interface IPostService
    {
        Task<PostDto> CreateAsync(long authorId, CreatePostRequest request, CancellationToken cancellationToken);
        Task<PostDto> GetByIdAsync(long postId, long viewerId, CancellationToken cancellationToken);
        Task<PostDto> UpdateAsync(long postId, long currentUserId, UpdatePostRequest request, CancellationToken cancellationToken);
        Task DeleteAsync(long postId, long currentUserId, CancellationToken cancellationToken);
        Task<ShareLinkDto> GetShareLinkAsync(long postId, long viewerId, CancellationToken cancellationToken);
        Task<PagedResult<PostDto>> GetSavedAsync(long userId, int page, int pageSize, CancellationToken cancellationToken);
        Task<PagedResult<PostDto>> GetFeedAsync(long userId, int page, int pageSize, CancellationToken cancellationToken);
        Task<PagedResult<PostDto>> GetDiscoveryFeedAsync(long userId, int page, int pageSize, CancellationToken cancellationToken);
    }
}
