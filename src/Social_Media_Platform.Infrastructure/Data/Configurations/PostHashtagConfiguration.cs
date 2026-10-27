using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Infrastructure.Data.Configurations
{
    public sealed class PostHashtagConfiguration : IEntityTypeConfiguration<PostHashtag>
    {
        public void Configure(EntityTypeBuilder<PostHashtag> builder)
        {
            builder.HasIndex(link => new
            {
                link.PostId,
                link.HashtagId
            }).IsUnique();

            builder.HasOne(link => link.Post)
                .WithMany(post => post.PostHashtags)
                .HasForeignKey(link => link.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(link => link.Hashtag)
                .WithMany(hashtag => hashtag.PostHashtags)
                .HasForeignKey(link => link.HashtagId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
