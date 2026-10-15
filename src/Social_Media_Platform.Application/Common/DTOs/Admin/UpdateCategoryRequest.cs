namespace Social_Media_Platform.Application.Common.DTOs.Admin
{
    public class UpdateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
