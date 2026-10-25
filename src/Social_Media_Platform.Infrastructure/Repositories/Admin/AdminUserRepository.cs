using Microsoft.EntityFrameworkCore;
using Social_Media_Platform.Application.DTOS.Admin;
using Social_Media_Platform.Application.Interfaces.IRepositories.Admin;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;
using Social_Media_Platform.Infrastructure.Data;

namespace Social_Media_Platform.Infrastructure.Repositories.Admin
{
    public class AdminUserRepository : IAdminUserRepository
    {
        private readonly Social_Media_PlatformDbContext context;
        public AdminUserRepository(Social_Media_PlatformDbContext context) => this.context = context;


        public async Task<User?> GetByIdAsync(long id) =>
          await context.Users
                .Include(u => u.Role)
                .Include(u => u.Profile)
                .FirstOrDefaultAsync(u => u.Id == id);

        public async Task<long?> GetRoleIdByNameAsync(RoleName roleName)
        {
            var roleId = await context.Roles
                .Where(role => role.Name == roleName)
                .Select(role => (long?)role.Id).FirstOrDefaultAsync();
            return roleId;
        }

        public async Task<List<AdminUserListItemDto>> GetUsersAsync() =>
            await context.Users
            .AsNoTracking()
            .Select(user => new AdminUserListItemDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.Profile != null ? user.Profile.DisplayName : null,
                Role = user.Role.Name,
                Status = user.Status ?? UserStatus.Active,
                IsVerified = user.IsVerified,
            })
            .ToListAsync();

        public async Task UpdateAsync(User user)
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
    }
}
