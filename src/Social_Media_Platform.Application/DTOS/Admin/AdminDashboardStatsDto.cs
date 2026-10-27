namespace Social_Media_Platform.Application.DTOS.Admin
{
    public class AdminDashboardStatsDto
    {
        public UserStatisticsDto Users { get; init; } = new();

        public ContentStatisticsDto Content { get; init; } = new();

        public ReportStatisticsDto Reports { get; init; } = new();

        public StreamStatisticsDto Streams { get; init; } = new();

        public DateTime GeneratedAt { get; init; }
    }


    public sealed class UserStatisticsDto
    {
        public int Total { get; init; }

        public int Active { get; init; }

        public int Suspended { get; init; }

        public int Blocked { get; init; }

        public int Verified { get; init; }
    }

    public sealed class ContentStatisticsDto
    {
        public int TotalPosts { get; init; }

        public int TotalComments { get; init; }

        public int TotalLikes { get; init; }

        public int TotalFollows { get; init; }
    }

    public sealed class ReportStatisticsDto
    {
        public int Total { get; init; }

        public int Pending { get; init; }

        public int Reviewed { get; init; }

        public int Actioned { get; init; }

        public int Dismissed { get; init; }
    }

    public sealed class StreamStatisticsDto
    {
        public int Total { get; init; }

        public int Scheduled { get; init; }

        public int Live { get; init; }

        public int Ended { get; init; }

        public int Cancelled { get; init; }
    }
}
