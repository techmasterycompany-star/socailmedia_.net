using System.ComponentModel.DataAnnotations;

namespace Social_Media_Platform.Application.DTOs.Auth
{
    public sealed class ForgotPasswordRequest
    {
        [Required, EmailAddress]
        public string Email { get; init; } = string.Empty;
    }
}
