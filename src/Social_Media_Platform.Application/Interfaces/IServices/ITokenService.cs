using Social_Media_Platform.Application.DTOs.Auth;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;

namespace Social_Media_Platform.Application.Interfaces.IServices
{
    public interface ITokenService
    {
        TokenResponse GenerateAccessToken(User user, RoleName roleName);
        string GenerateSecureToken();
        string HashToken(string token);
    }
}
