using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

public sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ConfigureBaseEntity("notifications");
        builder.Property(notification => notification.RecipientId)
            .HasColumnName("recipient_id")
            .IsRequired();
        builder.Property(notification => notification.ActorId)
            .HasColumnName("actor_id")
            .IsRequired();
        builder.Property(notification => notification.Type)
            .HasColumnName("notification_type")
            .HasConversion(new EnumStringConverter<NotificationType>())
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(notification => notification.IsRead)
            .HasColumnName("is_read")
            .HasDefaultValue(false)
            .IsRequired();
        builder.HasIndex(notification => notification.RecipientId);
        builder.HasIndex(notification => notification.ActorId);
        builder.HasOne(notification => notification.Recipient)
            .WithMany(user => user.ReceivedNotifications)
            .HasForeignKey(notification => notification.RecipientId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
        builder.HasOne(notification => notification.Actor)
            .WithMany(user => user.SentNotifications)
            .HasForeignKey(notification => notification.ActorId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}
