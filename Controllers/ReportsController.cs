using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Reports & Analytics";
            ViewBag.CurrentRole = Request.Query["role"].FirstOrDefault() ?? "System Administrator";
            ViewBag.CurrentPage = "Reports";
            return View();
        }
    }
}
