namespace Social_Media_Platform.Infrastructure.Options
{
    public sealed class SmtpOptions
    {
        public string? Host { get; set; }
        public int Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? From { get; set; }
    }

}
