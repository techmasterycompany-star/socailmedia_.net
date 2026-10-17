using Social_Media_Platform.Application.Common.DTOs.Admin;
using Social_Media_Platform.Application.Common.Exceptions;
using Social_Media_Platform.Application.Interfaces.IRepositories.Admin;
using Social_Media_Platform.Application.Interfaces.IServices.Admin;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;
using System.Threading;

namespace Social_Media_Platform.Application.Services.Admin
{
    public class AdminUserService : IAdminUserService
    {
        private readonly IAdminUserRepository repo;
        public AdminUserService(IAdminUserRepository repo) => this.repo = repo;



        public async Task<CommandResult> AssignModeratorRoleAsync(long id)
        {
            var user = await repo.GetByIdAsync(id)
                 ?? throw new NotFoundException("user not found");

            if (user.Role.Name == RoleName.Admin)
            {
                throw new BadRequestException("Administrator accounts already have elevated privileges");
            }
            if (user.Role.Name == RoleName.Moderator)
            {
                throw new ConflictException("The user already has the moderator role");
            }

            var moderatorRoleId = await repo.GetRoleIdByNameAsync(RoleName.Moderator)
                 ?? throw new NotFoundException("The moderator role is not configured");

            user.RoleId = moderatorRoleId;

            await repo.UpdateAsync(user);
            return new CommandResult($"Moderator role granted to '{user.Username}'.", user.Id);

        }

        public async Task<AdminUserListItemDto> GetUserAsync(long id)
        {
            var user = await repo.GetByIdAsync(id)
                ?? throw new NotFoundException("user not found");

            return new AdminUserListItemDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                DisplayName = user.Profile?.DisplayName,
                Role = user.Role.Name,
                Status = user.Status ?? UserStatus.Active,
                IsVerified = user.IsVerified,
            };
        }      

        public async Task<List<AdminUserListItemDto>> GetUsersAsync() =>
                    await repo.GetUsersAsync();

        public async Task<CommandResult> RemoveModeratorRoleAsync(long id)
        {
            var user = await repo.GetByIdAsync(id)
                ?? throw new NotFoundException("user not found");

            if (user.Role.Name == RoleName.Admin)
            {
                throw new BadRequestException("Administrator accounts cannot be demoted");
            }

            if (user.Role.Name != RoleName.Moderator)
            {
                throw new ConflictException("The user does not have the moderator role");
            }

            var userRoleId = await repo.GetRoleIdByNameAsync(RoleName.User)
                 ?? throw new NotFoundException("The user role is not configured");

            user.RoleId = userRoleId;
            await repo.UpdateAsync(user);

            return new CommandResult($"Moderator role removed from '{user.Username}' ", user.Id);

        }

        public async Task<AdminUserListItemDto> UpdateUserStatusAsync(long id, UpdateUserStatusRequest request)
        {
            var user = await repo.GetByIdAsync(id)
                 ?? throw new NotFoundException("The requested user could not be found");

            if (user.Role.Name == RoleName.Admin && request.Status != UserStatus.Active)
            {
                throw new BadRequestException("Administrator accounts cannot be suspended or blocked");
            }

            var currentStatus = user.Status ?? UserStatus.Active;
            if (currentStatus == request.Status)
            {
                throw new ConflictException($"The account is already '{request.Status}'.");
            }

            user.Status = request.Status;
            await repo.UpdateAsync(user);

            return await GetUserAsync(user.Id);

        }
    }
}
