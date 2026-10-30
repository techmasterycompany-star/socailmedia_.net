using Social_Media_Platform.Application.Common.DTOs.UserManagement;

namespace Social_Media_Platform.Application.Interfaces.IRepositories;

public interface IUserManagementRepository
{
    Task<UserProfileResponse> UpdateProfileAsync(
        long userId,
        UpdateUserProfileRequest request,
        CancellationToken cancellationToken);

    Task BlockAsync(long blockerId, long blockedId, CancellationToken cancellationToken);

    Task UnblockAsync(long blockerId, long blockedId, CancellationToken cancellationToken);

}
