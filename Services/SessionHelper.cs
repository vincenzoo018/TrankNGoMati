// ============================================================
//  TrackNGo Mati — Session Authentication Middleware
//  Cookie-based session auth: stores UserId + Role
// ============================================================
using Microsoft.AspNetCore.Http;
using System;

namespace TrackNGoMati.Services
{
    public static class SessionHelper
    {
        public const string KEY_USER_ID   = "UserId";
        public const string KEY_USER_NAME = "UserName";
        public const string KEY_ROLE      = "UserRole";
        public const string KEY_DEPT      = "UserDept";
        public const string KEY_FULL_NAME = "FullName";

        // Role constants
        public const int ROLE_ADMIN   = 1; // Executive Administrator
        public const int ROLE_RECORDS = 2; // Records Officer / Receiving Clerk
        public const int ROLE_DEPT    = 3; // Department Head
        public const int ROLE_MAYOR   = 4; // Mayor
        public const int ROLE_CART    = 5; // CART / Compliance Officer

        public static void SetSession(IHttpContextAccessor ctx, int userId, string fullName, int role, string dept)
        {
            ctx.HttpContext!.Session.SetInt32(KEY_USER_ID,   userId);
            ctx.HttpContext!.Session.SetString(KEY_FULL_NAME, fullName);
            ctx.HttpContext!.Session.SetInt32(KEY_ROLE,      role);
            ctx.HttpContext!.Session.SetString(KEY_DEPT,     dept ?? "");
        }

        public static int? GetUserId(IHttpContextAccessor ctx)   => ctx.HttpContext?.Session.GetInt32(KEY_USER_ID);
        public static int? GetRole(IHttpContextAccessor ctx)     => ctx.HttpContext?.Session.GetInt32(KEY_ROLE);
        public static string GetFullName(IHttpContextAccessor ctx) => ctx.HttpContext?.Session.GetString(KEY_FULL_NAME) ?? "User";
        public static bool IsLoggedIn(IHttpContextAccessor ctx)  => ctx.HttpContext?.Session.GetInt32(KEY_USER_ID) != null;

        public static string GetAreaForRole(int role) => role switch
        {
            ROLE_ADMIN   => "Admin",
            ROLE_RECORDS => "Receiving",
            ROLE_DEPT    => "DepartmentHead",
            ROLE_MAYOR   => "Mayor",
            ROLE_CART    => "Cart",
            _            => "Admin"
        };
    }
}
