using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;

namespace Social_Media_Platform.Infrastructure.Data.Configurations
{
    public sealed class BlockConfiguration : IEntityTypeConfiguration<Block>
    {
        public void Configure(EntityTypeBuilder<Block> builder)
        {
            builder.HasOne(block => block.Blocker)
                .WithMany()
                .HasForeignKey(block => block.BlockerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(block => block.BlockedUser)
                .WithMany()
                .HasForeignKey(block => block.BlockedUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(block => new
            {
                block.BlockerId,
                block.BlockedUserId
            }).IsUnique();

            builder.ToTable(table =>
                table.HasCheckConstraint(
                    "CK_Blocks_NoSelfBlock",
                    "[BlockerId] <> [BlockedUserId]"));
        }
    }
}
