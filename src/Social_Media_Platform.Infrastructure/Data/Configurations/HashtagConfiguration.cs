using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Infrastructure.Data.Configurations
{
    public sealed class HashtagConfiguration : IEntityTypeConfiguration<Hashtag>
    {
        public void Configure(EntityTypeBuilder<Hashtag> builder)
        {
            builder.Property(hashtag => hashtag.NormalizedName)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(hashtag => hashtag.NormalizedName)
                .IsUnique();
        }
    }
}
