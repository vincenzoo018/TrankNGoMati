using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TrackNGoMati.Models;

namespace TrackNGoMati.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly TrackNgoDbContext _context;

        public FeedbackController(TrackNgoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index(string? trackingNumber)
        {
            ViewBag.TrackingNumber = trackingNumber;
            return View();
        }

        [HttpPost]
        public IActionResult Submit(string TrackingNumber, int Rating, string? Comments)
        {
            if (string.IsNullOrEmpty(TrackingNumber))
            {
                TempData["Error"] = "Tracking Number is required.";
                return RedirectToAction("Index");
            }

            var doc = _context.Documents.FirstOrDefault(d => d.TrackingNumber == TrackingNumber);
            if (doc == null)
            {
                TempData["Error"] = "Invalid Tracking Number. Please check your Reference Number and try again.";
                return RedirectToAction("Index", new { trackingNumber = TrackingNumber });
            }

            var feedback = new CitizenFeedback
            {
                TrackingNumber = TrackingNumber,
                Rating = Rating,
                Comments = Comments,
                DateSubmitted = DateTime.Now
            };

            _context.CitizenFeedbacks.Add(feedback);
            _context.SaveChanges();

            TempData["Success"] = "Thank you for your feedback! Your response has been recorded.";
            return RedirectToAction("Index");
        }
    }
}
