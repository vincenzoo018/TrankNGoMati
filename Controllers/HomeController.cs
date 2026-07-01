using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Redirect to login
            return RedirectToAction("Login", "Account");
        }
    }
}
