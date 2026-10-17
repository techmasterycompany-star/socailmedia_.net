namespace Social_Media_Platform.Domain.Entities;

public sealed class EmailVerificationToken : BaseEntity
{
    public long UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? UsedAt { get; set; }
    public User User { get; set; } = null!;
}


