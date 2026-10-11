namespace Social_Media_Platform.Domain.Entities;

public abstract class BaseEntity
{
    public long Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
