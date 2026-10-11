using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ConfigureBaseEntity("comments");
        builder.Property(comment => comment.PostId).HasColumnName("post_id").IsRequired();
        builder.Property(comment => comment.AuthorId).HasColumnName("author_id").IsRequired();
        builder.Property(comment => comment.Content)
            .HasColumnName("content")
            .HasColumnType("nvarchar(max)")
            .IsRequired();
        builder.HasIndex(comment => comment.PostId);
        builder.HasIndex(comment => comment.AuthorId);
        builder.HasOne(comment => comment.Post)
            .WithMany(post => post.Comments)
            .HasForeignKey(comment => comment.PostId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
        builder.HasOne(comment => comment.Author)
            .WithMany(user => user.Comments)
            .HasForeignKey(comment => comment.AuthorId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
