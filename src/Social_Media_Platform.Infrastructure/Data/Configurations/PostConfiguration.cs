using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

public sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ConfigureBaseEntity("posts");

        builder.HasQueryFilter(post => !post.IsDeleted);

        builder.Property(post => post.AuthorId)
            .HasColumnName("author_id")
            .IsRequired();

        builder.Property(post => post.Content)
            .HasColumnName("content")
            .HasColumnType("nvarchar(max)");

        builder.Property(post => post.Visibility)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(post => post.CategoryId)
            .HasColumnName("category_id");

        builder.HasIndex(post => post.AuthorId);

        builder.HasIndex(post => new
        {
            post.CreatedAt,
            post.Id
        });

        builder.HasOne(post => post.Author)
            .WithMany(user => user.Posts)
            .HasForeignKey(post => post.AuthorId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();

        builder.HasOne(post => post.Category)
            .WithMany()
            .HasForeignKey(post => post.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
