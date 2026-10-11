using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

internal static class EntityTypeBuilderExtensions
{
    public static EntityTypeBuilder<TEntity> ConfigureBaseEntity<TEntity>(
        this EntityTypeBuilder<TEntity> builder,
        string tableName)
        where TEntity : BaseEntity
    {
        builder.ToTable(tableName);
        builder.HasKey(entity => entity.Id);
        builder.Property(entity => entity.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();
        builder.Property(entity => entity.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();
        builder.Property(entity => entity.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        if (typeof(SoftDelete).IsAssignableFrom(typeof(TEntity)))
        {
            builder.Property(nameof(SoftDelete.IsDeleted))
                .HasColumnName("is_deleted")
                .HasDefaultValue(false)
                .IsRequired();
            builder.Property(nameof(SoftDelete.DeletedAt))
                .HasColumnName("deleted_at");
        }

        return builder;
    }
}
