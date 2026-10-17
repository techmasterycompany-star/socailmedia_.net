using System.ComponentModel.DataAnnotations;

namespace Social_Media_Platform.Application.DTOs.Auth
{
    public sealed class LoginRequest
    {
        [Required]
        public string Identifier { get; init; } = string.Empty;

        [Required]
        public string Password { get; init; } = string.Empty;
    }
}
