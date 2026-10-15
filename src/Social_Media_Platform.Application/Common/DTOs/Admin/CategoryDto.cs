namespace Social_Media_Platform.Application.Common.DTOs.Admin
{
    public class CategoryDto
    {
        public long Id { get; init; }

        public string Name { get; init; } = string.Empty;

        public string? Description { get; init; }

        public bool IsActive { get; init; }

        public DateTime CreatedAt { get; init; }

        public DateTime UpdatedAt { get; init; }
    }
}
