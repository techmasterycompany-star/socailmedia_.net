namespace Social_Media_Platform.Domain.Entities;

public sealed class Profile : BaseEntity
{
    public long UserId { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string? Bio { get; set; }

    public string? ProfilePhoto { get; set; }

    public bool IsPrivate { get; set; }

    public User User { get; set; } = null!;
}
