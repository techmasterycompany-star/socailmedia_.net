using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

public sealed class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.ConfigureBaseEntity("post_likes");
        builder.Property(like => like.PostId).HasColumnName("post_id").IsRequired();
        builder.Property(like => like.UserId).HasColumnName("user_id").IsRequired();
        builder.HasIndex(like => new { like.PostId, like.UserId }).IsUnique();
        builder.HasIndex(like => like.UserId);
        builder.HasOne(like => like.Post)
            .WithMany(post => post.Likes)
            .HasForeignKey(like => like.PostId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
        builder.HasOne(like => like.User)
            .WithMany(user => user.PostLikes)
            .HasForeignKey(like => like.UserId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
