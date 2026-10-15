using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(long userId, CancellationToken cancellationToken = default);
        Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
        void Update(RefreshToken token);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }


}
