using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            ViewData["Title"] = "Sign In";
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password, string role)
        {
            // Front-end only: redirect to dashboard with role
            return RedirectToAction("Index", "Dashboard", new { role = role ?? "System Administrator" });
        }
    }
}
