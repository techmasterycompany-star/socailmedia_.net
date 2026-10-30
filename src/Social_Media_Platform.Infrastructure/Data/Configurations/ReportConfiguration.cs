using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Infrastructure.Data.Configurations;

public sealed class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.ConfigureBaseEntity("reports");
        builder.Property(report => report.ReporterId).HasColumnName("reporter_id").IsRequired();
        builder.Property(report => report.TargetType)
            .HasColumnName("target_type")
            .HasConversion(new EnumStringConverter<ReportTargetType>())
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(report => report.TargetId).HasColumnName("target_id").IsRequired();
        builder.Property(report => report.Reason)
            .HasColumnName("reason")
            .HasColumnType("nvarchar(max)")
            .IsRequired();
        builder.Property(report => report.Status)
            .HasColumnName("status")
            .HasConversion(new NullableEnumStringConverter<ReportStatus>())
            .HasMaxLength(20);
        builder.Property(report => report.ReviewedById).HasColumnName("reviewed_by_id");
        builder.Property(report => report.DecisionNotes)
            .HasColumnName("decision_notes")
            .HasColumnType("nvarchar(max)");
        builder.Property(report => report.Resolution)
            .HasColumnName("resolution")
            .HasConversion(new NullableEnumStringConverter<ReportResolution>())
            .HasMaxLength(30);
        builder.HasIndex(report => report.ReporterId);
        builder.HasIndex(report => report.ReviewedById);
        builder.HasOne(report => report.Reporter)
            .WithMany(user => user.SubmittedReports)
            .HasForeignKey(report => report.ReporterId)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
        builder.HasOne(report => report.Reviewer)
            .WithMany(user => user.ReviewedReports)
            .HasForeignKey(report => report.ReviewedById)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
