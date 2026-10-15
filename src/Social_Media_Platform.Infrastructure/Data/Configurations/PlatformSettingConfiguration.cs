using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Infrastructure.Data.Configurations
{
    public sealed class PlatformSettingConfiguration : IEntityTypeConfiguration<PlatformSetting>
    {
        public void Configure(EntityTypeBuilder<PlatformSetting> builder)
        {
            builder.ConfigureBaseEntity("platform_settings");
            builder.Property(setting => setting.Key)
                .HasColumnName("key")
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(setting => setting.Value)
                .HasColumnName("value")
                .HasColumnType("nvarchar(max)")
                .IsRequired();
            builder.Property(setting => setting.Description)
                .HasColumnName("description")
                .HasColumnType("nvarchar(max)");
            builder.Property(setting => setting.IsPublic)
                .HasColumnName("is_public")
                .HasDefaultValue(false)
                .IsRequired();
            builder.HasIndex(setting => setting.Key).IsUnique();
        }
    }
}
