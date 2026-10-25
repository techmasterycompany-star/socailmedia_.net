using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Application.Interfaces.IServices;
using Social_Media_Platform.Application.Options;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social_Media_Platform.Application.Services.Posts
{
    public sealed class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IHashtagRepository _hashtagRepository;
        private readonly IStorageService _storage;
        private readonly IUploadFileValidator _fileValidator;
        private readonly PostsMediaOptions _options;
        private readonly ILogger<PostService> _logger;

        public PostService(IPostRepository postRepository, IHashtagRepository hashtagRepository,
            IStorageService storage, IUploadFileValidator fileValidator, IOptions<PostsMediaOptions> options,
            ILogger<PostService> logger)
        {
            _postRepository = postRepository;
            _hashtagRepository = hashtagRepository;
            _storage = storage;
            _fileValidator = fileValidator;
            _options = options.Value;
            _logger = logger;
        }

        public async Task<PostDto> CreateAsync(long authorId, CreatePostRequest request, CancellationToken cancellationToken)
        {
            var media = request.Media ?? [];
            ValidatePostContent(request.Text, media.Count);
            await ValidateCategoryAsync(request.CategoryId, cancellationToken);

            var uploadedMedia = await UploadMediaAsync(media, cancellationToken);
            var now = DateTime.UtcNow;
            var post = new Post
            {
                AuthorId = authorId,
                Content = NormalizeText(request.Text),
                Visibility = request.Visibility,
                CategoryId = request.CategoryId,
                CreatedAt = now,
                UpdatedAt = now
            };

            for (var index = 0; index < uploadedMedia.Count; index++)
            {
                uploadedMedia[index].Post = post;
                uploadedMedia[index].DisplayOrder = index;
                post.Media.Add(uploadedMedia[index]);
            }

            try
            {
                await _hashtagRepository.AttachHashtagsAsync(post, post.Content, cancellationToken);
                await _postRepository.AddAsync(post, cancellationToken);
                await _postRepository.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                await CleanupUploadedMediaAsync(uploadedMedia, "post creation failed");
                throw;
            }

            return await _postRepository.GetDtoByIdAsync(post.Id, authorId, cancellationToken)
                ?? throw new NotFoundException("The post was created but could not be reloaded.");
        }

        public async Task<PostDto> GetByIdAsync(long postId, long viewerId, CancellationToken cancellationToken)
        {
            return await _postRepository.GetDtoByIdAsync(postId, viewerId, cancellationToken)
                ?? throw new NotFoundException("Post not found.");
        }

        public async Task<PostDto> UpdateAsync(long postId, long currentUserId, UpdatePostRequest request, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdForUpdateAsync(postId, cancellationToken)
                ?? throw new NotFoundException("Post not found.");

            if (!await CanViewAsync(post, currentUserId, cancellationToken))
                throw new NotFoundException("Post not found.");

            if (post.AuthorId != currentUserId)
                throw new UnauthorizedAccessException("You can only edit your own posts.");

            if (request.CategoryId != post.CategoryId)
                await ValidateCategoryAsync(request.CategoryId, cancellationToken);

            var media = request.Media ?? [];
            var removeMediaIds = request.RemoveMediaIds?.Distinct().ToArray() ?? [];

            var mediaToRemove = post.Media.Where(item => removeMediaIds.Contains(item.Id)).ToList();

            if (mediaToRemove.Count != removeMediaIds.Length)
                throw new ValidationException("One or more media IDs do not belong to this post.");

            var finalMediaCount = post.Media.Count - mediaToRemove.Count + media.Count;
            ValidatePostContent(request.Text, finalMediaCount);

            var uploadedMedia = await UploadMediaAsync(media, cancellationToken);

            foreach (var med in mediaToRemove)
                post.Media.Remove(med);

            var nextDisplayOrder = post.Media.Count == 0 ? 0 : post.Media.Max(media => media.DisplayOrder) + 1;

            foreach (var med in uploadedMedia)
            {
                med.Post = post;
                med.DisplayOrder = nextDisplayOrder++;
                post.Media.Add(med);
            }

            var index = 0;
            foreach (var med in post.Media.OrderBy(media => media.DisplayOrder).ToList())
                med.DisplayOrder = index++;

            post.Content = NormalizeText(request.Text);
            post.Visibility = request.Visibility;
            post.CategoryId = request.CategoryId;
            post.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _hashtagRepository.AttachHashtagsAsync(post, post.Content, cancellationToken);
                await _postRepository.SaveChangesAsync(cancellationToken);
            }
            catch
            {
                await CleanupUploadedMediaAsync(uploadedMedia, "post update failed");
                throw;
            }

            foreach (var med in mediaToRemove)
            {
                try
                {
                    await _storage.DeleteAsync(med.PublicId, GetResourceType(med.MediaType));
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception,
                        "Failed to delete removed post media {MediaId}; Cloudinary public ID {PublicId} needs cleanup.",
                        med.Id, med.PublicId);
                }
            }

            return await _postRepository.GetDtoByIdAsync(postId, currentUserId, cancellationToken) ?? throw new NotFoundException("Post not found.");
        }

        public async Task DeleteAsync(long postId, long currentUserId, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdForUpdateAsync(postId, cancellationToken)
                ?? throw new NotFoundException("Post not found.");

            if (!await CanViewAsync(post, currentUserId, cancellationToken))
                throw new NotFoundException("Post not found.");

            if (post.AuthorId != currentUserId)
                throw new UnauthorizedAccessException("You can only delete your own posts.");

            post.IsDeleted = true;
            post.DeletedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;

            await _postRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<ShareLinkDto> GetShareLinkAsync(long postId, long viewerId, CancellationToken cancellationToken)
        {
            var post = await _postRepository.GetByIdAsync(postId, cancellationToken)
                ?? throw new NotFoundException("Post not found.");

            if (!await CanViewAsync(post, viewerId, cancellationToken))
                throw new NotFoundException("Post not found.");

            return new ShareLinkDto($"{_options.ClientBaseUrl.TrimEnd('/')}/posts/{post.Id}");
        }

        public async Task<PagedResult<PostDto>> GetSavedAsync(long userId, int page, int pageSize, CancellationToken cancellationToken)
        {
            (page, pageSize) = NormalizePagination(page, pageSize);
            var (posts, totalCount) = await _postRepository.GetAccessibleSavedPageAsync(userId, page, pageSize, cancellationToken);

            return PagedResult<PostDto>.Create(posts, page, pageSize, totalCount);
        }

        public async Task<PagedResult<PostDto>> GetFeedAsync(long userId, int page, int pageSize, CancellationToken cancellationToken)
        {
            (page, pageSize) = NormalizePagination(page, pageSize);
            var (posts, totalCount) = await _postRepository.GetFeedPageAsync(userId, page, pageSize, cancellationToken);

            return PagedResult<PostDto>.Create(posts, page, pageSize, totalCount);
        }

        public async Task<PagedResult<PostDto>> GetDiscoveryFeedAsync(long userId, int page, int pageSize, CancellationToken cancellationToken)
        {
            (page, pageSize) = NormalizePagination(page, pageSize);
            var (posts, totalCount) = await _postRepository.GetDiscoveryPageAsync(userId, page, pageSize, cancellationToken);

            return PagedResult<PostDto>.Create(posts, page, pageSize, totalCount);
        }

        private async Task<List<PostMedia>> UploadMediaAsync(IReadOnlyCollection<IFormFile>? files, CancellationToken cancellationToken)
        {
            var uploaded = new List<PostMedia>();

            try
            {
                foreach (var file in files ?? Array.Empty<IFormFile>())
                {
                    var validated = await _fileValidator.ValidateAsync(file, cancellationToken);
                    var fileName = $"{Guid.NewGuid():N}{validated.Extension}";

                    await using var stream = file.OpenReadStream();
                    var result = validated.Kind == MediaType.Video
                        ? await _storage.UploadVideoAsync(stream, fileName, "posts", isPrivate: false)
                        : await _storage.UploadImageAsync(stream, fileName, "posts", isPrivate: false);

                    if (string.IsNullOrWhiteSpace(result.Url))
                        throw new InvalidOperationException("Cloudinary did not return a public media URL.");

                    uploaded.Add(new PostMedia
                    {
                        PublicId = result.PublicId,
                        MediaType = validated.Kind == MediaType.Video ? MediaType.Video : MediaType.Image,
                        DeliveryType = result.DeliveryType,
                        MediaUrl = result.Url,
                        ContentType = validated.ContentType,
                        SizeBytes = validated.SizeBytes,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    });
                }

                return uploaded;
            }
            catch
            {
                await CleanupUploadedMediaAsync(uploaded, "upload batch failed");
                throw;
            }
        }

        private async Task CleanupUploadedMediaAsync(IEnumerable<PostMedia> mediaItems, string reason)
        {
            foreach (var media in mediaItems)
            {
                try
                {
                    await _storage.DeleteAsync(media.PublicId, GetResourceType(media.MediaType));
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Failed to clean up media after {Reason}. Cloudinary public ID: {PublicId}.",
                        reason, media.PublicId);
                }
            }
        }

        private async Task<bool> CanViewAsync(Post post, long viewerId, CancellationToken cancellationToken)
        {
            if (post.IsDeleted || post.IsHidden || post.Author is null || post.Author.IsDeleted)
                return false;

            if (post.AuthorId == viewerId)
                return true;

            if (await _postRepository.IsBlockedEitherWayAsync(viewerId, post.AuthorId, cancellationToken))
                return false;

            var acceptedFollower = await _postRepository.IsAcceptedFollowerAsync(viewerId, post.AuthorId, cancellationToken);

            if (post.Author.Profile?.IsPrivate == true && !acceptedFollower)
                return false;

            return post.Visibility switch
            {
                PostVisibility.Public => true,
                PostVisibility.FollowersOnly => acceptedFollower,
                _ => false
            };
        }

        private async Task ValidateCategoryAsync(long? categoryId, CancellationToken cancellationToken)
        {
            if (categoryId.HasValue && !await _postRepository.CategoryExistsAsync(categoryId.Value, cancellationToken))
                throw new ValidationException("The selected category does not exist or is inactive.");
        }

        private static void ValidatePostContent(string? text, int mediaCount)
        {
            if (mediaCount > 10)
                throw new ValidationException("A post can contain at most 10 media items.");

            if (string.IsNullOrWhiteSpace(text) && mediaCount == 0)
                throw new ValidationException("A post must contain text or at least one media item.");
        }

        private static (int Page, int PageSize) NormalizePagination(int page, int pageSize)
        {
            pageSize = Math.Clamp(pageSize, 1, 100);
            page = Math.Clamp(page, 1, int.MaxValue / pageSize);
            return (page, pageSize);
        }

        private static string GetResourceType(MediaType mediaType) => mediaType == MediaType.Video ? "video" : "image";
        private static string? NormalizeText(string? text) => string.IsNullOrWhiteSpace(text) ? null : text.Trim();
    }

}
