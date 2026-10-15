using System.ComponentModel.DataAnnotations;

namespace Social_Media_Platform.Application.DTOs.Auth
{
    public sealed class VerifyEmailRequest
    {
        [Required, EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be exactly 6 digits.")]
        public string Otp { get; init; } = string.Empty;
    }
}
