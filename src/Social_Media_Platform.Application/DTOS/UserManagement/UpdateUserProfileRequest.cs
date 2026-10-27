using System.ComponentModel.DataAnnotations;

namespace Social_Media_Platform.Application.Common.DTOs.UserManagement;

public sealed record UpdateUserProfileRequest
{
    [StringLength(50, MinimumLength = 3)]
    public string? Username { get; init; }

    [StringLength(100, MinimumLength = 1)]
    public string? DisplayName { get; init; }

    [StringLength(2000)]
    public string? Bio { get; init; }
    public string? ProfilePhoto { get; init; }
    public bool? IsPrivate { get; init; }
}
