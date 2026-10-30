using System.Security.Claims;
namespace Social_Media_Platform.API.Helpers
{
    public static class ClaimsHelper
    {
        public static long GetAuthenticatedUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? user.FindFirst("sub")?.Value
                           ?? user.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out long userId))
            {
                throw new UnauthorizedAccessException("User is not authenticated or user ID is invalid");
            }

            return userId;
        }
    }
}

