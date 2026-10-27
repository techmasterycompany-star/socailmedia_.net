using System.ComponentModel.DataAnnotations;

namespace Social_Media_Platform.Application.DTOS.Post
{
    public sealed class SearchHashtagsRequest : IValidatableObject
    {
        public string? Q { get; init; }
        public int Limit { get; init; } = 20;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Limit is < 1 or > 100)
                yield return new ValidationResult("Limit must be between 1 and 100.", [nameof(Limit)]);
        }
    }
}
