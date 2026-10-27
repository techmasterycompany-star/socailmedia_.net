using Social_Media_Platform.Application.DTOs.Auth;

namespace Social_Media_Platform.Application.Interfaces.IServices
{
    public interface IStorageService
    {
        Task<CloudinaryUploadResult> UploadImageAsync(Stream file, string fileName, string folder, bool isPrivate = true);

        Task<CloudinaryUploadResult> UploadVideoAsync(Stream file, string fileName, string folder, bool isPrivate = true);

        Task DeleteAsync(string publicId, string resourceType);
    }
}
