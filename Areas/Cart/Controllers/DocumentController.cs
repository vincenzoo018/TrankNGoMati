using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackNGoMati.Filters;
using TrackNGoMati.Models;
using TrackNGoMati.Services;
using System.Linq;
using System;

namespace TrackNGoMati.Areas.Cart.Controllers
{
    [Area("Cart")]
    [RequireLogin(SessionHelper.ROLE_CART)]
    public class DocumentController : Controller
    {
        private readonly TrackNgoDbContext _context;
        private readonly SmsService _sms;

        public DocumentController(TrackNgoDbContext context, SmsService sms)
        {
            _context = context;
            _sms = sms;
        }

        public IActionResult Index(string? search = null)
        {
            ViewData["Title"] = "ARTA Monitoring Dashboard";
            ViewBag.CurrentPage = "Document";
            
            // CART monitors Step 3 specifically for clearing, but can see others
            var query = _context.Documents.Where(d => d.CurrentStepIndex == 3 || d.IsEscalated).AsQueryable();
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
        
        [HttpPost]
        public IActionResult Clear(string trackingNumber)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.TrackingNumber == trackingNumber);
            if (doc == null) return Json(new { success = false, message = "Document not found" });

            if (doc.CurrentStepIndex != 3)
                return Json(new { success = false, message = "Document is not in the CART stage." });

            var userId = HttpContext.Session.GetInt32(SessionHelper.KEY_USER_ID) ?? 0;

            // FSM Transition to Mayor (Step 4)
            doc.CurrentStepIndex = 4;
            doc.LastUpdated = DateTime.Now;

            _context.AuditTrailEntries.Add(new AuditTrailEntry
            {
                DocumentId = doc.Id,
                UserId = userId,
                Action = "Cleared by CART",
                Details = "CART reviewed the document for compliance and forwarded to Mayor.",
                Timestamp = DateTime.Now,
                Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"
            });

            _context.SaveChanges();
            
            if (!string.IsNullOrEmpty(doc.ContactNumber))
                _sms.Send(doc.Id, doc.ContactNumber, doc.SubmittedBy, $"TrackNGo Mati: Your document ({trackingNumber}) was cleared by CART and sent to the Mayor.", "CART Cleared");

            return Json(new { success = true, redirectUrl = "/Cart/Document" });
        }
        [HttpPost]
        public IActionResult FlagIssue(string trackingNumber)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.TrackingNumber == trackingNumber);
            if (doc == null) return Json(new { success = false, message = "Document not found" });

            var userId = HttpContext.Session.GetInt32(SessionHelper.KEY_USER_ID) ?? 0;

            doc.IsEscalated = true;
            doc.LastUpdated = DateTime.Now;

            _context.AuditTrailEntries.Add(new AuditTrailEntry
            {
                DocumentId = doc.Id,
                UserId = userId,
                Action = "Flagged by CART",
                Details = "CART flagged this document for potential ARTA compliance issues.",
                Timestamp = DateTime.Now,
                Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"
            });

            _context.SaveChanges();
            
            if (!string.IsNullOrEmpty(doc.ContactNumber))
                _sms.Send(doc.Id, doc.ContactNumber, doc.SubmittedBy, $"TrackNGo Mati: Your document ({trackingNumber}) was flagged by CART for review regarding ARTA compliance.", "Flagged");

            return Json(new { success = true, redirectUrl = "/Cart/Document" });
        }
    }
}
