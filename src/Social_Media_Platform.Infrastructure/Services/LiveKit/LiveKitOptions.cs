namespace Social_Media_Platform.Infrastructure.Services.LiveKit;

public sealed class LiveKitOptions
{
    public const string SectionName = "LiveKit";

    public string ApiKey { get; set; } = null!;
    public string ApiSecret { get; set; } = null!;
    public string Url { get; set; } = null!;
}