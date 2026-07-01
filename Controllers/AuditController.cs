using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Controllers
{
    public class AuditController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Audit Logs & Trails";
            ViewBag.CurrentRole = Request.Query["role"].FirstOrDefault() ?? "System Administrator";
            ViewBag.CurrentPage = "Audit";
            return View();
        }
    }
}
