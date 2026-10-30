using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

public sealed class ProfileConfiguration : IEntityTypeConfiguration<Profile>
{
    public void Configure(EntityTypeBuilder<Profile> builder)
    {
        builder.ConfigureBaseEntity("profiles");
        builder.Property(profile => profile.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(profile => profile.DisplayName)
            .HasColumnName("display_name")
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(profile => profile.Bio)
            .HasColumnName("bio")
            .HasColumnType("nvarchar(max)");
        builder.Property(profile => profile.ProfilePhoto)
            .HasColumnName("profile_photo")
            .HasMaxLength(2048);
        builder.Property(profile => profile.IsPrivate)
            .HasColumnName("is_private")
            .HasDefaultValue(false)
            .IsRequired();

        builder.HasIndex(profile => profile.UserId).IsUnique();
        builder.HasOne(profile => profile.User)
            .WithOne(user => user.Profile)
            .HasForeignKey<Profile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
