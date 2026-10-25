using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Social_Media_Platform.API.Helpers;
using Social_Media_Platform.Application.Common.DTOs;
using Social_Media_Platform.Application.DTOs.Auth;
using Social_Media_Platform.Application.Interfaces.IServices;
using System.Security.Claims;

namespace Social_Media_Platform.API.Controllers
{

    [ApiController]
    [Route("api/v1/auth")]
    public sealed class AuthController : ControllerBase
    {
        private const string RefreshCookieName = "__Host-refreshToken";

        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        {
            await _authService.RegisterAsync(request, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, ApiResponse<object>.SuccessResponse(new
            {
                message = "Registration successful. Check your email to verify your account."
            }));
        }

        [HttpPost("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token, CancellationToken cancellationToken)
        {
            await _authService.VerifyEmailAsync(email, token, cancellationToken);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                message = "Email verified successfully."
            }));
        }

        [HttpPost("resend-verification")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendVerificationEmail([FromBody] ResendVerificationEmailRequest request, CancellationToken cancellationToken)
        {
            await _authService.ResendVerificationEmailAsync(request.Email, cancellationToken);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                message = "If the account exists and requires verification, a verification email will be sent."
            }));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            var issued = await _authService.LoginAsync(request, HttpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);

            SetRefreshCookie(issued.RefreshToken);

            return Ok(ApiResponse<object>.SuccessResponse(issued.Response));
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies[RefreshCookieName];

            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return Unauthorized(ApiResponse<object>.FailResponse("Refresh token is missing."));
            }

            var issued = await _authService.RefreshAsync(refreshToken, HttpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);

            SetRefreshCookie(issued.RefreshToken);

            return Ok(ApiResponse<object>.SuccessResponse(issued.Response));
        }

        [HttpPost("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies[RefreshCookieName];

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                await _authService.LogoutAsync(refreshToken, HttpContext.Connection.RemoteIpAddress?.ToString(), cancellationToken);
            }

            Response.Cookies.Delete(RefreshCookieName, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                message = "Logged out successfully."
            }));
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken cancellationToken)
        {
            await _authService.ForgotPasswordAsync(request.Email, cancellationToken);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                message = "If the account exists, reset instructions will be sent."
            }));
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
        {
            await _authService.ResetPasswordAsync(request.Email, request.Otp, request.NewPassword, cancellationToken);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                message = "Password reset successfully."
            }));
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetAuthenticatedUserId();

            await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword, cancellationToken);

            return Ok(ApiResponse<object>.SuccessResponse(new
            {
                message = "Password changed successfully."
            }));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var userId = User.GetAuthenticatedUserId();
            var response = await _authService.GetCurrentUserAsync(userId, cancellationToken);

            return Ok(ApiResponse<CurrentUserResponse>.SuccessResponse(response));
        }

        private void SetRefreshCookie(string token)
        {
            Response.Cookies.Append(RefreshCookieName, token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                IsEssential = true,
                Path = "/",
                MaxAge = TimeSpan.FromDays(_configuration.GetValue<int>("Auth:RefreshTokenDays"))
            });
        }
    }


}
