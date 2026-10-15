using Social_Media_Platform.Application.DTOs.Auth;

namespace Social_Media_Platform.Application.Interfaces
{
    public interface IAuthService
    {
        Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
        Task VerifyEmailAsync(string email, string token, CancellationToken cancellationToken = default);
        Task ResendVerificationEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IssuedTokens> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken = default);
        Task<IssuedTokens> RefreshAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default);
        Task LogoutAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default);
        Task ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);
        Task ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);
        Task ChangePasswordAsync(long userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
        Task<CurrentUserResponse> GetCurrentUserAsync(long userId, CancellationToken cancellationToken = default);
    }


}
