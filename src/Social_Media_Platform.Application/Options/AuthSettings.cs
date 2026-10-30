using Microsoft.Extensions.Options;

namespace Social_Media_Platform.Application.Options
{
    public sealed class AuthSettings
    {
        public string ClientUrl { get; set; } = string.Empty;
        public int RefreshTokenDays { get; set; } = 7;
    }

    public sealed class PostsMediaOptions
    {
        public string ClientBaseUrl { get; set; } = "https://localhost:4200";
        public int SignedMediaUrlLifetimeMinutes { get; set; } = 5;
        public int ShareLinkLifetimeMinutes { get; set; } = 60;

        public TimeSpan SignedMediaUrlLifetime =>
            TimeSpan.FromMinutes(Math.Clamp(SignedMediaUrlLifetimeMinutes, 1, 15));

        public TimeSpan ShareLinkLifetime =>
            TimeSpan.FromMinutes(Math.Clamp(ShareLinkLifetimeMinutes, 1, 1440));
    }

    public sealed class ShareLinkOptions(IOptions<PostsMediaOptions> options)
    {
        private readonly PostsMediaOptions _options = options.Value;
        public string ClientBaseUrl => _options.ClientBaseUrl;
        public TimeSpan SignedMediaUrlLifetime => TimeSpan.FromMinutes(Math.Clamp(_options.SignedMediaUrlLifetimeMinutes, 1, 15));
        public TimeSpan ShareLinkLifetime => TimeSpan.FromMinutes(Math.Clamp(_options.ShareLinkLifetimeMinutes, 1, 1440));
    }
}
