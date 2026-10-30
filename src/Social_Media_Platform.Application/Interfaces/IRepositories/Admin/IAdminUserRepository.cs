using Social_Media_Platform.Application.DTOS.Admin;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Interfaces.IRepositories.Admin
{
    public interface IAdminUserRepository
    {
        Task<List<AdminUserListItemDto>> GetUsersAsync();
        Task<User?> GetByIdAsync(long id);
        Task<long?> GetRoleIdByNameAsync(RoleName roleName);
        Task UpdateAsync(User user);

    }
}
