using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Controllers
{
    public class DocumentsController : Controller
    {
        private string GetRole() => Request.Query["role"].FirstOrDefault() ?? "System Administrator";

        public IActionResult Index()
        {
            ViewData["Title"] = "Documents";
            ViewBag.CurrentRole = GetRole();
            ViewBag.CurrentPage = "Documents";
            return View();
        }

        public IActionResult Create()
        {
            ViewData["Title"] = "New Document";
            ViewBag.CurrentRole = GetRole();
            ViewBag.CurrentPage = "Documents";
            return View();
        }

        public IActionResult Details(string? refNo)
        {
            ViewData["Title"] = "Document Details";
            ViewBag.CurrentRole = GetRole();
            ViewBag.CurrentPage = "Documents";
            ViewBag.RefNo = refNo ?? "TNG-2026-0001";
            return View();
        }
    }
}
