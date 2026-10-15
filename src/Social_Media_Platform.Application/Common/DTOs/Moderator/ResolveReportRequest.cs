using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Common.DTOs.Moderator
{
    public class ResolveReportRequest
    {
        public ReportResolution Action { get; set; }

        public string? Notes { get; set; }
    }
}
