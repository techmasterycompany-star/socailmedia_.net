using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ConfigureBaseEntity("users");
        builder.Property(user => user.RoleId).HasColumnName("role_id").IsRequired();
        builder.Property(user => user.Email)
            .HasColumnName("email")
            .HasMaxLength(256)
            .IsRequired();
        builder.Property(user => user.Username)
            .HasColumnName("username")
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(user => user.Password)
            .HasColumnName("password")
            .HasMaxLength(512)
            .IsRequired();
        builder.Property(user => user.IsVerified)
            .HasColumnName("is_verified")
            .HasDefaultValue(false)
            .IsRequired();
        builder.Property(user => user.Status)
            .HasColumnName("status")
            .HasConversion(new NullableEnumStringConverter<UserStatus>())
            .HasMaxLength(20);

        builder.HasIndex(user => user.Email).IsUnique();
        builder.HasIndex(user => user.Username).IsUnique();
        builder.HasIndex(user => user.RoleId);

        builder.HasOne(user => user.Role)
            .WithMany(role => role.Users)
            .HasForeignKey(user => user.RoleId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder.HasQueryFilter(user => !user.IsDeleted);
    }
}
