using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.DTOS.Admin
{
    public sealed class AdminUserListItemDto
    {
        public long Id { get; init; }

        public string Username { get; init; } = string.Empty;

        public string Email { get; init; } = string.Empty;

        public string? DisplayName { get; init; }

        public RoleName Role { get; init; }

        public UserStatus Status { get; init; }

        public bool IsVerified { get; init; }
    }
}
