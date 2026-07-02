using Microsoft.AspNetCore.Mvc;
using TrackNGoMati.Models;
using System.Linq;

namespace TrackNGoMati.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DocumentController : Controller
    {
        private readonly TrackNgoDbContext _context;

        public DocumentController(TrackNgoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search = null)
        {
            ViewData["Title"] = "Document";
            ViewBag.CurrentPage = "Document";
            
            var query = _context.Documents.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.TrackingNumber.Contains(search) || d.Title.Contains(search));
            }

            return View(query.ToList());
        }

        public IActionResult Details(string? id)
        {
            ViewData["Title"] = "Document Details";
            ViewBag.CurrentPage = "Document";
            var doc = _context.Documents.FirstOrDefault(d => d.TrackingNumber == id);
            return View(doc);
        }
    }
}
