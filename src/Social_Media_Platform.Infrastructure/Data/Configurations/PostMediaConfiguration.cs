using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

public sealed class PostMediaConfiguration : IEntityTypeConfiguration<PostMedia>
{
    public void Configure(EntityTypeBuilder<PostMedia> builder)
    {
        builder.ConfigureBaseEntity("post_media");

        builder.Property(media => media.PostId)
            .HasColumnName("post_id")
            .IsRequired();

        builder.Property(media => media.MediaType)
            .HasColumnName("media_type")
            .HasConversion(new EnumStringConverter<MediaType>())
            .HasMaxLength(10)
            .IsRequired();

        builder.Property(media => media.ContentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(media => media.SizeBytes)
            .IsRequired();

        builder.Property(media => media.DisplayOrder)
            .IsRequired();

        builder.HasIndex(media => new
        {
            media.PostId,
            media.DisplayOrder
        }).IsUnique();

        builder.HasOne(media => media.Post)
            .WithMany(post => post.Media)
            .HasForeignKey(media => media.PostId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}