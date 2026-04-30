using System;
using System.Linq;
using System.Security.Claims;

namespace Insurancesys.web.Helper
{
    public static class UserProfileClaims
    {
        public static string GetEmail(this ClaimsPrincipal? user)
        {
            return user?.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        }

        public static string GetFirstName(this ClaimsPrincipal? user)
        {
            return user?.FindFirstValue("FirstName") ?? string.Empty;
        }

        public static string GetLastName(this ClaimsPrincipal? user)
        {
            return user?.FindFirstValue("LastName") ?? string.Empty;
        }

        public static string GetDisplayName(this ClaimsPrincipal? user)
        {
            var displayName = user?.FindFirstValue("DisplayName");
            if (!string.IsNullOrWhiteSpace(displayName))
                return displayName.Trim();

            var first = (user?.GetFirstName() ?? string.Empty).Trim();
            var last = (user?.GetLastName() ?? string.Empty).Trim();
            var combined = string.Join(" ", new[] { first, last }.Where(s => !string.IsNullOrWhiteSpace(s)));
            if (!string.IsNullOrWhiteSpace(combined))
                return combined;

            return user?.Identity?.Name ?? string.Empty;
        }

        public static string GetRoleName(this ClaimsPrincipal? user)
        {
            return user?.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
        }

        public static string GetAvatarLetter(this ClaimsPrincipal? user, string fallback = "?")
        {
            var name = GetDisplayName(user);
            if (string.IsNullOrWhiteSpace(name))
                return fallback;

            return name.Trim().Substring(0, 1).ToUpperInvariant();
        }
    }
}

