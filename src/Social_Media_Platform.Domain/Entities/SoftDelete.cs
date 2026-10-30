namespace Social_Media_Platform.Domain.Entities;

public abstract class SoftDelete : BaseEntity
{
    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }
}
