using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Domain.Entities;

public sealed class Role : BaseEntity
{
    public RoleName Name { get; set; }

    public string? Description { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}
