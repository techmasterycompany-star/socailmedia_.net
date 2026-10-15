namespace Social_Media_Platform.Application.DTOs.Auth
{
    public sealed class CurrentUserResponse
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public bool IsVerified { get; set; }
        public string? Status { get; set; }
        public string? Role { get; set; }
        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public string? ProfilePhoto { get; set; }
        public bool IsPrivate { get; set; }
    }
}
