using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Infrastructure.Data.Configurations
{
    public sealed class SavedPostConfiguration : IEntityTypeConfiguration<SavedPost>
    {
        public void Configure(EntityTypeBuilder<SavedPost> builder)
        {
            builder.HasIndex(saved => new
            {
                saved.UserId,
                saved.PostId
            }).IsUnique();

            builder.HasOne(saved => saved.User)
                .WithMany()
                .HasForeignKey(saved => saved.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(saved => saved.Post)
                .WithMany(post => post.SavedBy)
                .HasForeignKey(saved => saved.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
