using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Social_Media_Platform.Application.DTOs.Auth;
using Social_Media_Platform.Application.Interfaces.IRepositories;
using Social_Media_Platform.Application.Interfaces.IServices;
using Social_Media_Platform.Application.Options;
using Social_Media_Platform.Domain.Entities;
using Social_Media_Platform.Domain.Enums;
using System.Security.Cryptography;

namespace Social_Media_Platform.Application.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _rolesRepo;
        private readonly IRefreshTokenRepository _refreshTokensRepo;
        private readonly IOneTimeTokenRepository _oneTimeTokensRepo;
        private readonly IPasswordService _passwordService;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly AuthSettings _settings;

        public AuthService(
            IUserRepository users,
            IRoleRepository roles,
            IRefreshTokenRepository refreshTokens,
            IOneTimeTokenRepository oneTimeTokens,
            IPasswordService passwords,
            ITokenService tokens,
            IEmailService email,
            IOptions<AuthSettings> settings)
        {
            _userRepo = users;
            _rolesRepo = roles;
            _refreshTokensRepo = refreshTokens;
            _oneTimeTokensRepo = oneTimeTokens;
            _passwordService = passwords;
            _tokenService = tokens;
            _emailService = email;
            _settings = settings.Value;
        }

        public async Task RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            var email = request.Email.Trim().ToLowerInvariant();
            var username = request.Username.Trim();

            if (await _userRepo.EmailExistsAsync(email, cancellationToken))
            {
                throw new InvalidOperationException("Email already exists.");
            }

            if (await _userRepo.UsernameExistsAsync(username, cancellationToken))
            {
                throw new InvalidOperationException("Username already exists.");
            }

            var role = await _rolesRepo.GetByNameAsync(RoleName.User, cancellationToken)
                ?? throw new InvalidOperationException("Default User role is not configured.");

            var user = new User
            {
                Email = email,
                Username = username,
                Password = _passwordService.HashPassword(request.Password),
                RoleId = role.Id,
                IsVerified = false,
                Status = UserStatus.Active,
                Profile = new Profile
                {
                    DisplayName = request.DisplayName.Trim(),
                    IsPrivate = false
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepo.AddAsync(user, cancellationToken);
            await _userRepo.SaveChangesAsync(cancellationToken);


            var otp = GenerateEmailOtp();

            await _oneTimeTokensRepo.RevokeEmailVerificationTokensAsync(user.Id, cancellationToken);

            await _oneTimeTokensRepo.AddEmailVerificationTokenAsync(new EmailVerificationToken
            {
                UserId = user.Id,
                TokenHash = _tokenService.HashToken(otp),
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, cancellationToken);

            await _oneTimeTokensRepo.SaveChangesAsync(cancellationToken);

            var body = EmailTemplateBuilder.VerificationEmail(otp);
            await _emailService.SendAsync(email, "Verify your email", body, cancellationToken);

        }

        public async Task VerifyEmailAsync(string email, string otp, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByEmailAsync(email.Trim().ToLowerInvariant(), cancellationToken)
                ?? throw new InvalidOperationException("Invalid or expired verification code.");

            if (user.IsVerified)
            {
                throw new InvalidOperationException("Email is already verified.");
            }

            if (otp.Length != 6 || !otp.All(char.IsDigit))
            {
                throw new InvalidOperationException("Invalid or expired verification code.");
            }

            var stored = await _oneTimeTokensRepo.GetEmailVerificationTokenAsync(
                user.Id,
                _tokenService.HashToken(otp),
                cancellationToken)
                ?? throw new InvalidOperationException("Invalid or expired verification code.");

            user.IsVerified = true;
            user.UpdatedAt = DateTime.UtcNow;
            stored.UsedAt = DateTime.UtcNow;

            _userRepo.Update(user);

            await _userRepo.SaveChangesAsync(cancellationToken);
            await _oneTimeTokensRepo.SaveChangesAsync(cancellationToken);
        }

        public async Task ResendVerificationEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var user = await _userRepo.GetByEmailAsync(normalizedEmail, cancellationToken);

            if (user is null || user.IsVerified || user.IsDeleted || user.Status != UserStatus.Active)
            {
                return;
            }

            await _oneTimeTokensRepo.RevokeEmailVerificationTokensAsync(user.Id, cancellationToken);

            var otp = GenerateEmailOtp();

            await _oneTimeTokensRepo.AddEmailVerificationTokenAsync(new EmailVerificationToken
            {
                UserId = user.Id,
                TokenHash = _tokenService.HashToken(otp),
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, cancellationToken);

            await _oneTimeTokensRepo.SaveChangesAsync(cancellationToken);

            var body = EmailTemplateBuilder.VerificationEmail(otp);
            await _emailService.SendAsync(user.Email, "Your email verification code", body, cancellationToken);
        }

        public async Task<IssuedTokens> LoginAsync(LoginRequest request, string? ipAddress, CancellationToken cancellationToken = default)
        {
            var identifier = request.Identifier.Trim();

            var user = identifier.Contains('@')
                ? await _userRepo.GetByEmailAsync(identifier.ToLowerInvariant(), cancellationToken)
                : await _userRepo.GetByUsernameAsync(identifier, cancellationToken);

            if (user is null || !_passwordService.VerifyPassword(request.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            EnsureEligible(user);

            if (!user.IsVerified)
            {
                throw new InvalidOperationException("Email verification is required.");
            }

            var response = _tokenService.GenerateAccessToken(user, user.Role.Name);
            var rawRefreshToken = _tokenService.GenerateSecureToken();

            await _refreshTokensRepo.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                TokenHash = _tokenService.HashToken(rawRefreshToken),
                ExpiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays),
                CreatedByIp = ipAddress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, cancellationToken);

            await _refreshTokensRepo.SaveChangesAsync(cancellationToken);

            return new IssuedTokens(response, rawRefreshToken);
        }

        public async Task<IssuedTokens> RefreshAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default)
        {
            var stored = await _refreshTokensRepo.GetByHashAsync(_tokenService.HashToken(refreshToken), cancellationToken)
                ?? throw new UnauthorizedAccessException("Invalid refresh token.");

            if (stored.IsRevoked)
            {
                await RevokeAllTokensAsync(stored.UserId, ipAddress, cancellationToken);

                throw new UnauthorizedAccessException("Refresh token reuse detected.");
            }

            if (stored.IsExpired)
            {
                throw new UnauthorizedAccessException("Refresh token expired.");
            }

            EnsureEligible(stored.User);

            if (!stored.User.IsVerified)
            {
                throw new UnauthorizedAccessException("Email verification is required.");
            }

            var response = _tokenService.GenerateAccessToken(stored.User, stored.User.Role.Name);
            var rawNewToken = _tokenService.GenerateSecureToken();

            var newToken = new RefreshToken
            {
                UserId = stored.UserId,
                TokenHash = _tokenService.HashToken(rawNewToken),
                ExpiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenDays),
                CreatedByIp = ipAddress,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            stored.RevokedAt = DateTime.UtcNow;
            stored.RevokedByIp = ipAddress;
            stored.ReplacedByTokenId = newToken.Id;
            stored.UpdatedAt = DateTime.UtcNow;

            _refreshTokensRepo.Update(stored);

            await _refreshTokensRepo.AddAsync(newToken, cancellationToken);
            await _refreshTokensRepo.SaveChangesAsync(cancellationToken);

            return new IssuedTokens(response, rawNewToken);
        }

        public async Task LogoutAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default)
        {
            var stored = await _refreshTokensRepo.GetByHashAsync(_tokenService.HashToken(refreshToken), cancellationToken);

            if (stored is null || stored.IsRevoked)
            {
                return;
            }

            stored.RevokedAt = DateTime.UtcNow;
            stored.RevokedByIp = ipAddress;
            stored.UpdatedAt = DateTime.UtcNow;

            _refreshTokensRepo.Update(stored);

            await _refreshTokensRepo.SaveChangesAsync(cancellationToken);
        }


        public async Task ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();
            var user = await _userRepo.GetByEmailAsync(normalizedEmail, cancellationToken);

            if (user is null || !user.IsVerified || user.IsDeleted || user.Status != UserStatus.Active)
            {
                return;
            }

            await _oneTimeTokensRepo.RevokePasswordResetTokensAsync(user.Id, cancellationToken);

            var otp = GenerateEmailOtp();

            await _oneTimeTokensRepo.AddPasswordResetTokenAsync(new PasswordResetToken
            {
                UserId = user.Id,
                TokenHash = _tokenService.HashToken(otp),
                ExpiresAt = DateTime.UtcNow.AddMinutes(10),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, cancellationToken);

            await _oneTimeTokensRepo.SaveChangesAsync(cancellationToken);

            var body = EmailTemplateBuilder.PasswordResetEmail(otp);
            await _emailService.SendAsync(user.Email, "Your password reset code", body, cancellationToken);
        }



        public async Task ResetPasswordAsync(string email, string otp, string newPassword, CancellationToken cancellationToken = default)
        {
            var normalizedEmail = email.Trim().ToLowerInvariant();

            var user = await _userRepo.GetByEmailAsync(normalizedEmail, cancellationToken)
                ?? throw new InvalidOperationException("Invalid or expired reset code.");

            if (otp.Length != 6 || !otp.All(char.IsDigit))
            {
                throw new InvalidOperationException("Invalid or expired reset code.");
            }

            var stored = await _oneTimeTokensRepo.GetPasswordResetTokenAsync(
                user.Id,
                _tokenService.HashToken(otp),
                cancellationToken)
                ?? throw new InvalidOperationException("Invalid or expired reset code.");

            if (user.IsDeleted || user.Status != UserStatus.Active || !user.IsVerified)
            {
                throw new InvalidOperationException("This account cannot reset its password.");
            }

            user.Password = _passwordService.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;
            stored.UsedAt = DateTime.UtcNow;

            _userRepo.Update(user);

            await _userRepo.SaveChangesAsync(cancellationToken);

            await _oneTimeTokensRepo.RevokePasswordResetTokensAsync(user.Id, cancellationToken);
            await _oneTimeTokensRepo.SaveChangesAsync(cancellationToken);

            await RevokeAllTokensAsync(user.Id, null, cancellationToken);
        }


        public async Task ChangePasswordAsync(long userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByIdAsync(userId, cancellationToken)
                ?? throw new KeyNotFoundException("User not found.");

            EnsureEligible(user);

            if (!_passwordService.VerifyPassword(currentPassword, user.Password))
            {
                throw new UnauthorizedAccessException("Current password is incorrect.");
            }

            user.Password = _passwordService.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            _userRepo.Update(user);

            await _userRepo.SaveChangesAsync(cancellationToken);

            await RevokeAllTokensAsync(user.Id, null, cancellationToken);
        }

        public async Task<CurrentUserResponse> GetCurrentUserAsync(long userId, CancellationToken cancellationToken = default)
        {
            var user = await _userRepo.GetByIdAsync(userId, cancellationToken)
                ?? throw new KeyNotFoundException("User not found.");

            EnsureEligible(user);

            return new CurrentUserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                IsVerified = user.IsVerified,
                Status = user.Status?.ToString(),
                Role = user.Role?.Name.ToString(),
                DisplayName = user.Profile?.DisplayName,
                Bio = user.Profile?.Bio,
                ProfilePhoto = user.Profile?.ProfilePhoto,
                IsPrivate = user.Profile?.IsPrivate ?? false
            };
        }


        private static void EnsureEligible(User user)
        {
            if (user.IsDeleted || user.Status != UserStatus.Active)
            {
                throw new UnauthorizedAccessException("This account is not eligible to authenticate.");
            }
        }

        private async Task RevokeAllTokensAsync(long userId, string? ipAddress, CancellationToken cancellationToken)
        {
            var activeTokens = await _refreshTokensRepo.GetActiveByUserIdAsync(userId, cancellationToken);

            foreach (var token in activeTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;
                token.UpdatedAt = DateTime.UtcNow;

                _refreshTokensRepo.Update(token);
            }

            await _refreshTokensRepo.SaveChangesAsync(cancellationToken);
        }

        private static string GenerateEmailOtp()
        {
            return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        }
    }

}
