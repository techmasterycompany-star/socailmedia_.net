using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Common.DTOs.Admin
{
    public sealed class UpdateUserStatusRequest
    {
        public UserStatus Status { get; set; }
    }
}
