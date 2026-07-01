using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Controllers
{
    public class UsersController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "User Management";
            ViewBag.CurrentRole = Request.Query["role"].FirstOrDefault() ?? "System Administrator";
            ViewBag.CurrentPage = "Users";
            return View();
        }
    }
}
