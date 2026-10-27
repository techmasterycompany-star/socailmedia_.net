using Microsoft.AspNetCore.Http;
using Social_Media_Platform.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Social_Media_Platform.Application.DTOS.Post
{
    public sealed class UpdatePostRequest : IValidatableObject
    {
        [StringLength(5000)]
        public string? Text { get; init; }

        public PostVisibility Visibility { get; init; } = PostVisibility.Public;
        public long? CategoryId { get; init; }
        public List<IFormFile> Media { get; init; } = [];
        public List<long> RemoveMediaIds { get; init; } = [];

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Media.Count > 10)
                yield return new ValidationResult("A request can upload at most 10 media items.", [nameof(Media)]);
            if (RemoveMediaIds.Any(id => id <= 0) || RemoveMediaIds.Count != RemoveMediaIds.Distinct().Count())
                yield return new ValidationResult("Media IDs to remove must be positive and unique.", [nameof(RemoveMediaIds)]);
        }
    }
}
