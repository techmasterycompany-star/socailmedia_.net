using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

public sealed class BlockConfiguration : IEntityTypeConfiguration<Block>
{
    public void Configure(EntityTypeBuilder<Block> builder)
    {
        builder.ConfigureBaseEntity("blocks");
        builder.Property(block => block.BlockerId).HasColumnName("blocker_id").IsRequired();
        builder.Property(block => block.BlockedId).HasColumnName("blocked_id").IsRequired();
        builder.HasIndex(block => new { block.BlockerId, block.BlockedId }).IsUnique();
        builder.HasIndex(block => block.BlockedId);
        builder.HasOne(block => block.Blocker)
            .WithMany()
            .HasForeignKey(block => block.BlockerId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
        builder.HasOne(block => block.Blocked)
            .WithMany()
            .HasForeignKey(block => block.BlockedId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
