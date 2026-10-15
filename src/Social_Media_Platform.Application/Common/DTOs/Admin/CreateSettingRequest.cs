namespace Social_Media_Platform.Application.Common.DTOs.Admin
{
    public class CreateSettingRequest
    {
        public string Key { get; set; } = string.Empty;

        public string Value { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsPublic { get; set; }
    }
}
