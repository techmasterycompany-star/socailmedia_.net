using Social_Media_Platform.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social_Media_Platform.Application.DTOs.Auth
{
    public sealed class ResetPasswordRequest
    {
        [Required, EmailAddress, StringLength(256)]
        public string Email { get; init; } = string.Empty;

        [Required, RegularExpression(@"^\d{6}$", ErrorMessage = "OTP must be exactly 6 digits.")]
        public string Otp { get; init; } = string.Empty;

        [Required, StringLength(128, MinimumLength = 8)]
        [RegularExpression(@"^(?=\S+$)(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{8,128}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character, with no whitespace.")]
        public string NewPassword { get; init; } = string.Empty;

        [Required, Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; init; } = string.Empty;
    }
}
