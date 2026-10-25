using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Application.DTOs.Auth;
using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Application.Interfaces.IServices;
using Social_Media_Platform.Infrastructure.Options;

namespace Social_Media_Platform.Infrastructure.Services
{
    public sealed class CloudinaryStorageService : IStorageService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryStorageService(IOptions<CloudinarySettings> settings)
        {
            var account = new Account(settings.Value.CloudName, settings.Value.ApiKey, settings.Value.ApiSecret);

            _cloudinary = new Cloudinary(account);
        }

        public async Task<CloudinaryUploadResult> UploadImageAsync(Stream file, string fileName, string folder, bool isPrivate = true)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(fileName, file),
                Folder = folder,
                Type = isPrivate ? "authenticated" : "upload"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                throw new BadRequestException($"Image upload failed: {result.Error.Message}");
            }

            return new CloudinaryUploadResult
            {
                PublicId = result.PublicId,
                ResourceType = "image",
                DeliveryType = isPrivate ? "authenticated" : "upload",
                Url = isPrivate ? null : result.SecureUrl?.ToString()
            };
        }

        public async Task<CloudinaryUploadResult> UploadVideoAsync(Stream file, string fileName, string folder, bool isPrivate = true)
        {
            var uploadParams = new VideoUploadParams
            {
                File = new FileDescription(fileName, file),
                Folder = folder,
                Type = isPrivate ? "authenticated" : "upload"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                throw new BadRequestException($"Video upload failed: {result.Error.Message}");
            }

            return new CloudinaryUploadResult
            {
                PublicId = result.PublicId,
                ResourceType = "video",
                DeliveryType = isPrivate ? "authenticated" : "upload",
                Url = isPrivate ? null : result.SecureUrl?.ToString()
            };
        }

        public async Task DeleteAsync( string publicId, string resourceType)
        {
            var deleteParams = new DeletionParams(publicId)
            {
                ResourceType = resourceType == "video"? ResourceType.Video: ResourceType.Image
            };

            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error != null || result.Result != "ok" && result.Result != "not found")
            {
                throw new InvalidOperationException("Failed to delete media from Cloudinary.");
            }
        }
    }
}
