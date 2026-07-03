using Microsoft.AspNetCore.Mvc;
using TrackNGoMati.Models;
using TrackNGoMati.Services;
using System.Linq;
using Microsoft.AspNetCore.Http;
using TrackNGoMati.Filters;

namespace TrackNGoMati.Controllers
{
    public class ProfileController : Controller
    {
        private readonly TrackNgoDbContext _context;

        public ProfileController(TrackNgoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32(SessionHelper.KEY_USER_ID);
            if (!userId.HasValue) return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(userId.Value);
            ViewBag.Users = _context.Users.Where(u => u.Id != userId.Value && u.IsActive).ToList();
            
            // Layout to use depends on role
            if (user.Role == SessionHelper.ROLE_ADMIN) ViewBag.Layout = "~/Areas/Admin/Views/Shared/_AdminLayout.cshtml";
            else if (user.Role == SessionHelper.ROLE_MAYOR) ViewBag.Layout = "~/Areas/Mayor/Views/Shared/_MayorLayout.cshtml";
            else if (user.Role == SessionHelper.ROLE_DEPT) ViewBag.Layout = "~/Areas/DepartmentHead/Views/Shared/_DepartmentHeadLayout.cshtml";
            else if (user.Role == SessionHelper.ROLE_CART) ViewBag.Layout = "~/Areas/Cart/Views/Shared/_CartLayout.cshtml";
            else ViewBag.Layout = "~/Areas/Receiving/Views/Shared/_ReceivingLayout.cshtml";

            return View(user);
        }

        [HttpPost]
        public IActionResult UpdateOutOfOffice(bool IsOutOfOffice, int? DelegatedUserId)
        {
            var userId = HttpContext.Session.GetInt32(SessionHelper.KEY_USER_ID);
            if (!userId.HasValue) return RedirectToAction("Login", "Account");

            var user = _context.Users.Find(userId.Value);
            if (user != null)
            {
                user.IsOutOfOffice = IsOutOfOffice;
                user.DelegatedUserId = DelegatedUserId;
                _context.SaveChanges();
                TempData["Success"] = "Profile settings updated successfully.";
            }

            return RedirectToAction("Index");
        }
    }
}
