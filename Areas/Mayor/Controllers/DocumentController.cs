using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrackNGoMati.Filters;
using TrackNGoMati.Models;
using TrackNGoMati.Services;
using System.Linq;
using System;

namespace TrackNGoMati.Areas.Mayor.Controllers
{
    [Area("Mayor")]
    [RequireLogin(SessionHelper.ROLE_MAYOR)]
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
            ViewData["Title"] = "Executive Approvals";
            ViewBag.CurrentPage = "Document";
            
            // FSM Step 4: Mayor
            var query = _context.Documents.Where(d => d.CurrentStepIndex == 4).AsQueryable();
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
        public IActionResult Approve(string trackingNumber, string signatureData)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.TrackingNumber == trackingNumber);
            if (doc == null) return Json(new { success = false, message = "Document not found" });

            if (doc.CurrentStepIndex != 4)
                return Json(new { success = false, message = "Document is not in the Mayor stage." });

            var userId = HttpContext.Session.GetInt32(SessionHelper.KEY_USER_ID) ?? 0;
            
            _context.DigitalSignatures.Add(new DigitalSignature
            {
                DocumentId = doc.Id,
                SignedByUserId = userId,
                SignatureImagePath = signatureData, // Storing base64 string directly
                SignatureHash = "sha256:generated-hash-placeholder", // Real hash logic here
                ActionType = "Approved",
                IsVerified = true,
                SignedAt = DateTime.Now
            });

            // FSM Transition to Completed/Receiving for release (Step 5)
            doc.CurrentStepIndex = 5;
            doc.LastUpdated = DateTime.Now;

            _context.AuditTrailEntries.Add(new AuditTrailEntry
            {
                DocumentId = doc.Id,
                UserId = userId,
                Action = "Approved",
                Details = "Mayor approved and signed the document. It is now ready for release.",
                Timestamp = DateTime.Now,
                Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1"
            });

            _context.SaveChanges();
            
            if (!string.IsNullOrEmpty(doc.ContactNumber))
                _sms.Send(doc.Id, doc.ContactNumber, doc.SubmittedBy, $"TrackNGo Mati: Great news! Your document ({trackingNumber}) was approved by the Mayor. You may now claim it.", "Approved");

            return Json(new { success = true, redirectUrl = "/Mayor/Document" });
        }
    }
}
