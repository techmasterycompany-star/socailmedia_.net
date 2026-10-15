using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Application.Interfaces
{
    public interface IOneTimeTokenRepository
    {
        Task AddEmailVerificationTokenAsync(EmailVerificationToken token, CancellationToken cancellationToken = default);
        Task<EmailVerificationToken?> GetEmailVerificationTokenAsync(long userId, string tokenHash, CancellationToken cancellationToken = default);
        Task RevokeEmailVerificationTokensAsync(long userId, CancellationToken cancellationToken = default);
        Task AddPasswordResetTokenAsync(PasswordResetToken token, CancellationToken cancellationToken = default);
        Task<PasswordResetToken?> GetPasswordResetTokenAsync(long userId, string tokenHash, CancellationToken cancellationToken = default);
        Task RevokePasswordResetTokensAsync(long userId, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }


}
