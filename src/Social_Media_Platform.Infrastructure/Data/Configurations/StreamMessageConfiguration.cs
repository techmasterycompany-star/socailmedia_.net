using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

public sealed class StreamMessageConfiguration : IEntityTypeConfiguration<StreamMessage>
{
    public void Configure(EntityTypeBuilder<StreamMessage> builder)
    {
        builder.ConfigureBaseEntity("stream_messages");
        builder.Property(message => message.StreamId).HasColumnName("stream_id").IsRequired();
        builder.Property(message => message.SenderId).HasColumnName("sender_id").IsRequired();
        builder.Property(message => message.MessageType)
            .HasColumnName("message_type")
            .HasConversion(new NullableEnumStringConverter<StreamMessageType>())
            .HasMaxLength(20);
        builder.Property(message => message.Content)
            .HasColumnName("content")
            .HasColumnType("nvarchar(max)");
        builder.HasIndex(message => message.StreamId);
        builder.HasIndex(message => message.SenderId);
        builder.HasOne(message => message.Stream)
            .WithMany(stream => stream.Messages)
            .HasForeignKey(message => message.StreamId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
        builder.HasOne(message => message.Sender)
            .WithMany(user => user.StreamMessages)
            .HasForeignKey(message => message.SenderId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
