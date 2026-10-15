namespace Social_Media_Platform.Application.DTOs.Auth
{
    public sealed record TokenResponse(string AccessToken, DateTime ExpiresAtUtc, long UserId, string Username, string Email, string Role);
    public sealed record IssuedTokens(TokenResponse Response, string RefreshToken);
}
