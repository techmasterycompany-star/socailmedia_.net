using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ConfigureBaseEntity("roles");
        builder.Property(role => role.Name)
            .HasColumnName("name")
            .HasConversion(new EnumStringConverter<RoleName>())
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(role => role.Description)
            .HasColumnName("description")
            .HasColumnType("nvarchar(max)");
        builder.HasIndex(role => role.Name).IsUnique();
    }
}
