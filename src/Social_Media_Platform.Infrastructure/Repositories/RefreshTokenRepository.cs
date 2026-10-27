using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.Infrastructure.Repositories
{
    public sealed class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly Social_Media_PlatformDbContext _context;

        public RefreshTokenRepository(Social_Media_PlatformDbContext context) => _context = context;

        public async Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        {
            return await _context.RefreshTokens
                .Include(token => token.User).ThenInclude(user => user.Role)
                .SingleOrDefaultAsync(token => token.TokenHash == tokenHash, cancellationToken);
        }

        public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(long userId, CancellationToken cancellationToken = default) {
            return await _context.RefreshTokens
                .Where(token => token.UserId == userId && token.RevokedAt == null && token.ExpiresAt > DateTime.UtcNow)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
        {
            await _context.RefreshTokens.AddAsync(token, cancellationToken);
        }

        public void Update(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
