using Social_Media_Platform.Application.DTOS.Admin;

namespace Social_Media_Platform.Application.Interfaces.IServices.Admin
{
    public interface IAdminUserService 
    {
        Task<List<AdminUserListItemDto>> GetUsersAsync();
        Task<AdminUserListItemDto> GetUserAsync(long id);
        Task<AdminUserListItemDto> UpdateUserStatusAsync(long id , UpdateUserStatusRequest request);
        Task<CommandResult> AssignModeratorRoleAsync(long id);
        Task<CommandResult> RemoveModeratorRoleAsync(long id);
    }
}
