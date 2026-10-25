namespace Social_Media_Platform.Application.DTOS.Admin
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
