using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

public sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ConfigureBaseEntity("posts");
        builder.Property(post => post.AuthorId).HasColumnName("author_id").IsRequired();
        builder.Property(post => post.Content)
            .HasColumnName("content")
            .HasColumnType("nvarchar(max)");
        builder.Property(post => post.IsPublic)
            .HasColumnName("is_public")
            .HasDefaultValue(true)
            .IsRequired();
        builder.HasIndex(post => post.AuthorId);
        builder.HasOne(post => post.Author)
            .WithMany(user => user.Posts)
            .HasForeignKey(post => post.AuthorId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
