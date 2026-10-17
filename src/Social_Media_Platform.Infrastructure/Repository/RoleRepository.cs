using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.Interfaces;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.Infrastructure.Repository
{
    public sealed class RoleRepository : IRoleRepository
    {
        private readonly Social_Media_PlatformDbContext _context;

        public RoleRepository(Social_Media_PlatformDbContext context) => _context = context;

        public Task<Role?> GetByNameAsync(RoleName name, CancellationToken cancellationToken = default)
        {
            return _context.Roles.FirstOrDefaultAsync(role => role.Name == name, cancellationToken);
        }
    }
}
