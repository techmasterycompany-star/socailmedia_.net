using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

public sealed class FollowConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.ConfigureBaseEntity("follows");
        builder.Property(follow => follow.FollowerId).HasColumnName("follower_id").IsRequired();
        builder.Property(follow => follow.FollowingId).HasColumnName("following_id").IsRequired();
        builder.Property(follow => follow.Status)
            .HasColumnName("status")
            .HasConversion(new EnumStringConverter<FollowStatus>())
            .HasMaxLength(20)
            .IsRequired();
        builder.HasIndex(follow => new { follow.FollowerId, follow.FollowingId }).IsUnique();
        builder.HasOne(follow => follow.Follower)
            .WithMany(user => user.OutgoingFollows)
            .HasForeignKey(follow => follow.FollowerId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
        builder.HasOne(follow => follow.Following)
            .WithMany(user => user.IncomingFollows)
            .HasForeignKey(follow => follow.FollowingId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
