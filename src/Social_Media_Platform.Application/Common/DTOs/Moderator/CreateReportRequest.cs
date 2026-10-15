
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Common.DTOs.Moderator
{
    public class CreateReportRequest
    {
        public ReportTargetType TargetType { get; set; }

        public long TargetId { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}
