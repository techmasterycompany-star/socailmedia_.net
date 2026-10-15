namespace Social_Media_Platform.Application.Options
{
    public sealed class AuthSettings
    {
        public string ClientUrl { get; set; } = string.Empty;
        public int RefreshTokenDays { get; set; } = 7;
    }

}
