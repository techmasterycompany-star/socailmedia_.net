using Microsoft.AspNetCore.Http;
using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Application.DTOS.Post;
using Social_Media_Platform.Application.Interfaces.IServices;
using Social_Media_Platform.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Social_Media_Platform.Application.Services.Posts
{
    public sealed class UploadFileValidator : IUploadFileValidator
    {
        private const long MaxImageBytes = 10L * 1024 * 1024;
        private const long MaxVideoBytes = 200L * 1024 * 1024;

        public async Task<ValidatedUpload> ValidateAsync(IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null || file.Length <= 0)
                throw new ValidationException("An empty file cannot be uploaded.");

            var declaredType = file.ContentType?.Split(';', 2)[0].Trim().ToLowerInvariant();
            var kind = declaredType switch
            {
                "image/jpeg" or "image/png" or "image/gif" or "image/webp" => MediaType.Image,
                "video/mp4" or "video/webm" => MediaType.Video,
                _ => throw new ValidationException("Unsupported media content type.")
            };

            var maximumSize = kind == MediaType.Image ? MaxImageBytes : MaxVideoBytes;
            if (file.Length > maximumSize)
                throw new ValidationException(kind == MediaType.Image
                    ? "Images must not exceed 10 MB."
                    : "Videos must not exceed 200 MB.");

            await using var stream = file.OpenReadStream();
            var header = new byte[Math.Min(16, (int)Math.Min(file.Length, 16))];
            var read = await stream.ReadAsync(header.AsMemory(0, header.Length), cancellationToken);
            if (!HasValidSignature(header.AsSpan(0, read), declaredType!))
                throw new  ValidationException("The file signature does not match its declared content type.");

            var extension = declaredType switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                "image/webp" => ".webp",
                "video/mp4" => ".mp4",
                "video/webm" => ".webm",
                _ => throw new ValidationException("Unsupported media content type.")
            };

            return new ValidatedUpload(kind, declaredType!, file.Length, extension);
        }

        private static bool HasValidSignature(ReadOnlySpan<byte> bytes, string contentType) => contentType switch
        {
            "image/jpeg" => bytes.Length >= 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF,
            "image/png" => bytes.Length >= 8 && bytes[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            "image/gif" => bytes.Length >= 6 && (bytes[..6].SequenceEqual("GIF87a"u8) || bytes[..6].SequenceEqual("GIF89a"u8)),
            "image/webp" => bytes.Length >= 12 && bytes[..4].SequenceEqual("RIFF"u8) && bytes.Slice(8, 4).SequenceEqual("WEBP"u8),
            "video/mp4" => bytes.Length >= 8 && bytes.Slice(4, 4).SequenceEqual("ftyp"u8),
            "video/webm" => bytes.Length >= 4 && bytes[..4].SequenceEqual(new byte[] { 0x1A, 0x45, 0xDF, 0xA3 }),
            _ => false
        };
    }
}
