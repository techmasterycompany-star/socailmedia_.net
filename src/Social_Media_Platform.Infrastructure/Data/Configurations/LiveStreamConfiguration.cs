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
            .HasConversion(new NullableEnumStringConverter<AudienceSetting>())
            .HasMaxLength(20);
        builder.Property(stream => stream.Status)
            .HasColumnName("status")
            .HasConversion(new NullableEnumStringConverter<LiveStreamStatus>())
            .HasMaxLength(20);
        builder.Property(stream => stream.RecordingUrl)
            .HasColumnName("recording_url")
            .HasMaxLength(2048);
        builder.HasIndex(stream => stream.HostId);
        builder.HasOne(stream => stream.Host)
            .WithMany(user => user.HostedStreams)
            .HasForeignKey(stream => stream.HostId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
