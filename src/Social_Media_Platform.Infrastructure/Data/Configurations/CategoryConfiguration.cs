using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Infrastructure.Data.Configurations
{

    public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ConfigureBaseEntity("categories");
            builder.Property(category => category.Name)
                .HasColumnName("name")
                .HasMaxLength(100)
                .IsRequired();
            builder.Property(category => category.Description)
                .HasColumnName("description")
                .HasColumnType("nvarchar(max)");
            builder.Property(category => category.IsActive)
                .HasColumnName("is_active")
                .HasDefaultValue(true)
                .IsRequired();
            builder.HasIndex(category => category.Name).IsUnique();
        }
    }
}
