namespace Social_Media_Platform.Infrastructure.Options
{
    public sealed class EmailOptions
    {
        public string? ClientUrl { get; set; }

        public SmtpOptions Smtp { get; set; } = new();
    }

}
