namespace Social_Media_Platform.Application.Common.DTOs.Admin
{
    public class PlatformSettingDto
    {
        public long Id { get; init; }

        public string Key { get; init; } = string.Empty;

        public string Value { get; init; } = string.Empty;

        public string? Description { get; init; }

        public bool IsPublic { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; init; }
    }
}
