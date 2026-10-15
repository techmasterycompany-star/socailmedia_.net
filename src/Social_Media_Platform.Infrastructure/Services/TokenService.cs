using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Social_Media_Platform.Application.DTOs.Auth;
using Social_Media_Platform.Application.Interfaces;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;
using Social_Media_Platform.Infrastructure.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Social_Media_Platform.Infrastructure.Services
{
    public sealed class TokenService : ITokenService
    {
        private readonly JwtSettings _settings;

        public TokenService(IOptions<JwtSettings> settings) => _settings = settings.Value;

        public TokenResponse GenerateAccessToken(User user, RoleName roleName)
        {
            var expiresAtUtc = DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, roleName.ToString()),
                new("is_verified", user.IsVerified.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(_settings.Issuer, _settings.Audience, claims, expires: expiresAtUtc, signingCredentials: credentials);

            return new TokenResponse(new JwtSecurityTokenHandler().WriteToken(jwt), expiresAtUtc, user.Id, user.Username, user.Email, roleName.ToString());
        }

        public string GenerateSecureToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        public string HashToken(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }

}
