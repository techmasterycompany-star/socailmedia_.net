namespace Social_Media_Platform.Application.DTOs.Auth
{
    public sealed class CloudinaryUploadResult
    {
        public string PublicId { get; init; } = string.Empty;

        public string ResourceType { get; init; } = string.Empty;

        public string DeliveryType { get; init; } = string.Empty;

        public string? Url { get; init; }
    }
}
