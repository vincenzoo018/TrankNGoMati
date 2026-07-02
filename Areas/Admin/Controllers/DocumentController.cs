using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackNGoMati.Filters;
using TrackNGoMati.Models;
using TrackNGoMati.Services;
using System.Linq;

namespace TrackNGoMati.Areas.Admin.Controllers
{
    [Area("Admin")]
    [RequireLogin(SessionHelper.ROLE_ADMIN)]
    public class DocumentController : Controller
    {
        private readonly TrackNgoDbContext _context;

        public DocumentController(TrackNgoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search = null)
        {
            ViewData["Title"] = "System Document Overview";
            ViewBag.CurrentPage = "Document";
            
            var query = _context.Documents.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.TrackingNumber.Contains(search) || d.Title.Contains(search));
            }

            return View(query.OrderByDescending(d => d.LastUpdated).ToList());
        }

        public IActionResult Details(string? id)
        {
            ViewData["Title"] = "Document Details";
            ViewBag.CurrentPage = "Document";
            var doc = _context.Documents
                .Include(d => d.AuditTrailEntries).ThenInclude(a => a.User)
                .Include(d => d.WorkflowTransitions)
                .FirstOrDefault(d => d.TrackingNumber == id);
                
            if (doc == null) return NotFound();
            return View(doc);
        }
    }
}
