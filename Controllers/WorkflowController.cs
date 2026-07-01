using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Controllers
{
    public class WorkflowController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Workflow";
            ViewBag.CurrentRole = Request.Query["role"].FirstOrDefault() ?? "System Administrator";
            ViewBag.CurrentPage = "Workflow";
            return View();
        }
    }
}
