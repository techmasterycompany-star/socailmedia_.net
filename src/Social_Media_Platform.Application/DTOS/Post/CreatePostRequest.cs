using Microsoft.AspNetCore.Http;
using Social_Media_Platform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social_Media_Platform.Application.DTOS.Post
{
    public sealed class CreatePostRequest : IValidatableObject
    {
        [StringLength(5000)]
        public string? Text { get; init; }

        public List<IFormFile> Media { get; init; } = [];
        public PostVisibility Visibility { get; init; } = PostVisibility.Public;
        public long? CategoryId { get; init; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Text) && Media.Count == 0)
                yield return new ValidationResult("A post must contain text or at least one media item.", [nameof(Text), nameof(Media)]);
            if (Media.Count > 10)
                yield return new ValidationResult("A post can contain at most 10 media items.", [nameof(Media)]);
        }
    }
}
