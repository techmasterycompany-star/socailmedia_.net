using System.ComponentModel.DataAnnotations;

namespace Social_Media_Platform.Application.DTOS.Post
{
    public sealed class CreateCommentRequest : IValidatableObject
    {
        [Required]
        [StringLength(2000)]
        public string Content { get; init; } = string.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Content))
                yield return new ValidationResult("Comment content cannot be empty.", [nameof(Content)]);
        }
    }
}
