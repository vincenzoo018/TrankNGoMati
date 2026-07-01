using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Controllers
{
    public class EscalationController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "ARTA Smart Escalation";
            ViewBag.CurrentRole = Request.Query["role"].FirstOrDefault() ?? "System Administrator";
            ViewBag.CurrentPage = "Escalation";
            return View();
        }
    }
}
