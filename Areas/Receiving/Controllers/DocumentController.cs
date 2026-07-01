using Microsoft.AspNetCore.Mvc;

namespace TrackNGoMati.Areas.Receiving.Controllers
{
    [Area("Receiving")]
    public class DocumentController : Controller
    {        public IActionResult Index()
        {
            ViewData["Title"] = "Document";
            ViewBag.CurrentPage = "Document";
            return View();
        }        public IActionResult Details(string? id)
        {
            ViewData["Title"] = "Document Details";
            ViewBag.CurrentPage = "Document";
            ViewBag.RefNo = id ?? "TNG-2026-0001";
            return View();
        }    }
}
