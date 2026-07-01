using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserManagementController : Controller
    {        public IActionResult Index()
        {
            ViewData["Title"] = "UserManagement";
            ViewBag.CurrentPage = "UserManagement";
            return View();
        }    }
}
