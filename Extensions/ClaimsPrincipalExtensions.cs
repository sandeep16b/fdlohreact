using System.Security.Claims;

namespace ReactAspNetApp.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetLoginUser(this ClaimsPrincipal user)
        {
            return user.Identity?.Name ?? string.Empty;
        }

        public static string GetLoginUserRole(this ClaimsPrincipal user)
        {
            if (user.IsInRole("FLDOH_Admin")) return "Admin";
            if (user.IsInRole("FLDOH_Facilitator")) return "Facilitator";
            if (user.IsInRole("FLDOH_Delegate")) return "Delegate";
            if (user.IsInRole("FLDOH_Sanitizer")) return "Sanitizer";
            return "User";
        }
    }
}
