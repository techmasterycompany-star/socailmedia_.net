using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.DTOS.Admin
{
    public sealed class UpdateUserStatusRequest
    {
        public UserStatus Status { get; set; }
    }
}
