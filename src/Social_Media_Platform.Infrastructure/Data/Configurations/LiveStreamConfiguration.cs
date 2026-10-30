using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

public sealed class LiveStreamConfiguration : IEntityTypeConfiguration<LiveStream>
{
    public void Configure(EntityTypeBuilder<LiveStream> builder)
    {
        builder.ConfigureBaseEntity("live_streams");
        builder.Property(stream => stream.HostId).HasColumnName("host_id").IsRequired();
        builder.Property(stream => stream.Title)
            .HasColumnName("title")
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(stream => stream.AudienceSetting)
            .HasColumnName("audience_setting")
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(stream => stream.RoomName)
            .HasColumnName("room_name")
            .HasMaxLength(150)
            .IsRequired();
        builder.HasIndex(stream => stream.RoomName)
            .IsUnique();
        builder.Property(stream => stream.Status)
            .HasColumnName("status")
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(stream => stream.RecordingUrl)
            .HasColumnName("recording_url")
            .HasMaxLength(2048);
        builder.HasIndex(stream => stream.HostId);
        builder.HasOne(stream => stream.Host)
            .WithMany(user => user.HostedStreams)
            .HasForeignKey(stream => stream.HostId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
        builder.Property(entity => entity.StartAt)
            .HasColumnName("start_at");
        builder.Property(entity => entity.EndedAt)
            .HasColumnName("ended_at");
    }
}
