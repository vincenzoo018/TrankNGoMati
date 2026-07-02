// ============================================================
//  TrackNGo Mati — Auth Filter
//  Blocks unauthenticated requests at controller level
// ============================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TrackNGoMati.Services;

namespace TrackNGoMati.Filters
{
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        private readonly int[] _allowedRoles;

        public RequireLoginAttribute(params int[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userId  = session.GetInt32(SessionHelper.KEY_USER_ID);
            var role    = session.GetInt32(SessionHelper.KEY_ROLE);

            if (userId == null)
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "" });
                return;
            }

            if (_allowedRoles.Length > 0 && role != null && !Array.Exists(_allowedRoles, r => r == role.Value))
            {
                // User is logged in but has wrong role — redirect to their correct dashboard
                var area = SessionHelper.GetAreaForRole(role.Value);
                context.Result = new RedirectToActionResult("Index", "Dashboard", new { area });
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
