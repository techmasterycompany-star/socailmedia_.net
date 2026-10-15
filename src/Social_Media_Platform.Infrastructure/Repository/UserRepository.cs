using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.Interfaces;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.Infrastructure.Repository
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly Social_Media_PlatformDbContext _context;

        public UserRepository(Social_Media_PlatformDbContext context) => _context = context;

        public Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return _context.Users.Include(user => user.Role).Include(user => user.Profile).FirstOrDefaultAsync(user => user.Id == id && !user.IsDeleted, cancellationToken);
        }

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) 
        {
            return _context.Users.Include(user => user.Role).FirstOrDefaultAsync(user => user.Email == email && !user.IsDeleted, cancellationToken);
        }

        public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return _context.Users.Include(user => user.Role).FirstOrDefaultAsync(user => user.Username == username && !user.IsDeleted, cancellationToken);
        }

        public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        {
            return _context.Users.AnyAsync(user => user.Email == email && !user.IsDeleted, cancellationToken);
        }

        public Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
        {
            return _context.Users.AnyAsync(user => user.Username == username && !user.IsDeleted, cancellationToken);
        }   

        public async Task AddAsync(User user, CancellationToken cancellationToken = default) 
        {
            await _context.Users.AddAsync(user, cancellationToken);
        }

        public void Update(User user)
        {
            _context.Users.Update(user);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
