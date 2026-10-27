using Microsoft.AspNetCore.Http;
using Social_Media_Platform.Application.DTOS.Post;

namespace Social_Media_Platform.Application.Interfaces.IServices
{
    public interface IUploadFileValidator
    {
        Task<ValidatedUpload> ValidateAsync(IFormFile file, CancellationToken cancellationToken);
    }

}
