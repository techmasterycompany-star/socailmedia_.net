using System.ComponentModel.DataAnnotations;

namespace Social_Media_Platform.Application.DTOS.Post
{
    public sealed class SearchUsersRequest : IValidatableObject
    {
        public string Q { get; init; } = string.Empty;
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Q))
                yield return new ValidationResult("Search query is required.", [nameof(Q)]);

            if (Page < 1)
                yield return new ValidationResult("Page must be greater than 0.", [nameof(Page)]);

            if (PageSize is < 1 or > 100)
                yield return new ValidationResult("Page size must be between 1 and 100.", [nameof(PageSize)]);
        }
    }
}
