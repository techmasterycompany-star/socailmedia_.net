using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

public sealed class ModerationLogConfiguration : IEntityTypeConfiguration<ModerationLog>
{
    public void Configure(EntityTypeBuilder<ModerationLog> builder)
    {
        builder.ConfigureBaseEntity("moderation_logs");
        builder.Property(log => log.ActorId).HasColumnName("actor_id").IsRequired();
        builder.Property(log => log.TargetType)
            .HasColumnName("target_type")
            .HasConversion(new EnumStringConverter<ModerationTargetType>())
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(log => log.TargetId).HasColumnName("target_id").IsRequired();
        builder.Property(log => log.Action)
            .HasColumnName("action")
            .HasConversion(new EnumStringConverter<ModerationAction>())
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(log => log.Reason)
            .HasColumnName("reason")
            .HasColumnType("nvarchar(max)");
        builder.HasIndex(log => log.ActorId);
        builder.HasIndex(log => new { log.TargetType, log.TargetId });
        builder.HasOne(log => log.Actor)
            .WithMany()
            .HasForeignKey(log => log.ActorId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
