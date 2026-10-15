using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.Interfaces;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.Infrastructure.Repository
{
    public sealed class OneTimeTokenRepository : IOneTimeTokenRepository
    {
        private readonly Social_Media_PlatformDbContext _context;

        public OneTimeTokenRepository(Social_Media_PlatformDbContext context) => _context = context;

        public async Task AddEmailVerificationTokenAsync(EmailVerificationToken token, CancellationToken cancellationToken = default)
        {
            await _context.EmailVerificationTokens.AddAsync(token, cancellationToken);
        }

        public Task<EmailVerificationToken?> GetEmailVerificationTokenAsync(long userId, string tokenHash, CancellationToken cancellationToken = default)
        {
            return _context.EmailVerificationTokens
                .FirstOrDefaultAsync(token => token.UserId == userId && token.TokenHash == tokenHash 
                && token.UsedAt == null && token.ExpiresAt > DateTime.UtcNow, cancellationToken);
        }

        public async Task RevokeEmailVerificationTokensAsync(long userId, CancellationToken cancellationToken = default)
        {
            var tokens = await _context.EmailVerificationTokens
                .Where(token => token.UserId == userId && token.UsedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var token in tokens)
                token.UsedAt = DateTime.UtcNow;
        }

        public async Task AddPasswordResetTokenAsync(PasswordResetToken token, CancellationToken cancellationToken = default)
        {
            await _context.PasswordResetTokens.AddAsync(token, cancellationToken);
        }

        public async Task<PasswordResetToken?> GetPasswordResetTokenAsync(long userId, string tokenHash, CancellationToken cancellationToken = default)
        {
            return await _context.PasswordResetTokens
                .FirstOrDefaultAsync(token => token.UserId == userId && token.TokenHash == tokenHash
                    && token.UsedAt == null && token.ExpiresAt > DateTime.UtcNow, cancellationToken);
        }

        public async Task RevokePasswordResetTokensAsync(long userId, CancellationToken cancellationToken = default)
        {
            var tokens = await _context.PasswordResetTokens
                .Where(token => token.UserId == userId && token.UsedAt == null)
                .ToListAsync(cancellationToken);

            foreach (var token in tokens) 
                token.UsedAt = DateTime.UtcNow;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) => _context.SaveChangesAsync(cancellationToken);
    }
}
