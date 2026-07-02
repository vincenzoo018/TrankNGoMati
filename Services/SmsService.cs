// ============================================================
//  TrackNGo Mati — SMS Notification Service (Mock)
//  Logs SMS records to the DB; does NOT send real SMS
// ============================================================
using System;
using TrackNGoMati.Models;

namespace TrackNGoMati.Services
{
    public class SmsService
    {
        private readonly TrackNgoDbContext _context;

        public SmsService(TrackNgoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Logs an SMS record in the SMSNotifications table.
        /// In production, replace the Console.WriteLine with your SMS gateway call.
        /// </summary>
        public void Send(int documentId, string recipientNumber, string recipientName, string message, string triggerEvent, int? recipientUserId = null)
        {
            // Normalize the mobile number
            recipientNumber = recipientNumber?.Trim() ?? "";

            var sms = new Smsnotification
            {
                DocumentId      = documentId,
                RecipientNumber = recipientNumber,
                RecipientName   = recipientName,
                MessageContent  = message.Length > 499 ? message[..499] : message,
                TriggerEvent    = triggerEvent,
                SentAt          = DateTime.Now,
                IsDelivered     = true,  // Mock: always "delivered"
                GatewayResponse = "MOCK_OK",
                RecipientUserId = recipientUserId
            };

            _context.Smsnotifications.Add(sms);
            _context.SaveChanges();

            // TODO: When real SMS gateway is configured, add API call here.
            // Example (Semaphore):
            // await _http.PostAsync("https://api.semaphore.co/api/v4/messages", ...);
            Console.WriteLine($"[SMS MOCK] To: {recipientNumber} | Msg: {message}");
        }

        /// <summary>
        /// Sends a document status-change notification to the client.
        /// </summary>
        public void NotifyStatusChange(Document doc, string newStatus)
        {
            if (string.IsNullOrEmpty(doc.ContactNumber)) return;

            var message = $"TrackNGo Mati: Your document '{doc.Title}' (Ref# {doc.TrackingNumber}) status has been updated to: {newStatus}. For inquiries, contact the Mayor's Office.";
            Send(doc.Id, doc.ContactNumber, doc.SubmittedBy, message, "StatusChange");
        }

        /// <summary>
        /// Sends an ARTA escalation alert to the compliance officer.
        /// </summary>
        public void NotifyEscalation(Document doc, string officerNumber, string officerName)
        {
            var elapsed  = (int)(DateTime.Now - doc.DateFiled).TotalDays;
            var message  = $"ARTA ALERT - TrackNGo: Document '{doc.Title}' (Ref# {doc.TrackingNumber}) has exceeded its {doc.ArtaprocessingDays}-day ARTA limit. Elapsed: {elapsed} days. Immediate action required.";
            Send(doc.Id, officerNumber, officerName, message, "ARTAEscalation");
        }
    }
}
