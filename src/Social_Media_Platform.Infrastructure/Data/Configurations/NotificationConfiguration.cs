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
        builder.Property(notification => notification.FollowId)
            .HasColumnName("follow_id");
        builder.Property(notification => notification.PostId)
            .HasColumnName("post_id");
        builder.Property(notification => notification.CommentId)
            .HasColumnName("comment_id");
        builder.Property(notification => notification.LiveStreamId)
            .HasColumnName("live_stream_id");
        builder.HasIndex(notification => new
        {
            notification.RecipientId,
            notification.IsRead,
            notification.CreatedAt
        });
        builder.HasIndex(notification => notification.ActorId);
        builder.HasIndex(notification => new
        {
            notification.RecipientId,
            notification.ActorId,
            notification.Type,
            notification.FollowId,
            notification.PostId,
            notification.CommentId,
            notification.LiveStreamId
        });
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
        builder.HasOne<Follow>()
            .WithMany()
            .HasForeignKey(notification => notification.FollowId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<Post>()
            .WithMany()
            .HasForeignKey(notification => notification.PostId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<Comment>()
            .WithMany()
            .HasForeignKey(notification => notification.CommentId)
            .OnDelete(DeleteBehavior.NoAction);
        builder.HasOne<LiveStream>()
            .WithMany()
            .HasForeignKey(notification => notification.LiveStreamId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
