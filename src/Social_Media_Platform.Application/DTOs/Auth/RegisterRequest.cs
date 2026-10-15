using System.ComponentModel.DataAnnotations;

namespace Social_Media_Platform.Application.DTOs.Auth
{
    public sealed class RegisterRequest
    {
        [Required, StringLength(150, MinimumLength = 2)]
        public string DisplayName { get; init; } = string.Empty;

        [Required, StringLength(50, MinimumLength = 3)]
        [RegularExpression(@"^[a-zA-Z0-9_.-]+$", ErrorMessage = "Username can contain only letters, numbers, underscores, dots, and hyphens. The @ symbol is not allowed.")]
        public string Username { get; init; } = string.Empty;

        [Required, EmailAddress, StringLength(256)]
        public string Email { get; init; } = string.Empty;

        [Required, StringLength(128, MinimumLength = 8)]
        [RegularExpression(@"^(?=\S+$)(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,128}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character, with no whitespace.")]
        public string Password { get; init; } = string.Empty;

        [Required, Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; init; } = string.Empty;
    }
}
