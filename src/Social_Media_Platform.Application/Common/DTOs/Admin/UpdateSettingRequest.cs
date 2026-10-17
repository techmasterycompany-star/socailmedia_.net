namespace Social_Media_Platform.Application.Common.DTOs.Admin
{
    public class UpdateSettingRequest
    {
        public string Value { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsPublic { get; set; }
    }
}
