using System;
using System.Linq;
using TrackNGoMati.Models;

namespace TrackNGoMati.Services
{
    public class WorkflowEngine
    {
        private readonly TrackNgoDbContext _context;
        private readonly SmsService _sms;

        public WorkflowEngine(TrackNgoDbContext context, SmsService sms)
        {
            _context = context;
            _sms = sms;
        }

        public bool TransitionDocument(string trackingNumber, int userId, string action, string remarks, int? targetUserId = null, string signatureData = null)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.TrackingNumber == trackingNumber);
            var user = _context.Users.Find(userId);

            if (doc == null || user == null) return false;
            
            // Check out-of-office delegation
            if (targetUserId.HasValue)
            {
                var target = _context.Users.Find(targetUserId.Value);
                if (target != null && target.IsOutOfOffice && target.DelegatedUserId.HasValue)
                {
                    targetUserId = target.DelegatedUserId.Value;
                    remarks = $"[Auto-Routed (Out of Office)] " + remarks;
                }
            }
            
            // Validate and process FSM transition
            var oldStep = doc.CurrentStepIndex;
            var oldOffice = doc.CurrentOfficeName;

            bool validTransition = false;
            string newStatusText = "";

            switch (action.ToUpper())
            {
                case "ENDORSE":
                    if (doc.CurrentStepIndex == 1 && user.Role == SessionHelper.ROLE_RECORDS) {
                        doc.CurrentStepIndex = 2;
                        doc.CurrentOfficeName = "Department Head";
                        doc.CurrentStatus = 1; // In Progress
                        newStatusText = "Endorsed";
                        validTransition = true;
                    }
                    break;

                case "ACCEPT":
                    if (doc.CurrentStepIndex == 2 && user.Role == SessionHelper.ROLE_DEPT) {
                        doc.CurrentStepIndex = 3;
                        doc.CurrentOfficeName = "CART";
                        newStatusText = "Accepted by Dept Head";
                        validTransition = true;
                    }
                    break;

                case "REVIEW":
                    if (doc.CurrentStepIndex == 3 && user.Role == SessionHelper.ROLE_CART) {
                        doc.CurrentStepIndex = 4;
                        doc.CurrentOfficeName = "Mayor's Office";
                        newStatusText = "Reviewed by CART";
                        validTransition = true;
                    }
                    break;

                case "APPROVE":
                    if (doc.CurrentStepIndex == 4 && (user.Role == SessionHelper.ROLE_MAYOR || user.Role == SessionHelper.ROLE_ADMIN)) {
                        doc.CurrentStepIndex = 5;
                        doc.CurrentOfficeName = "Completed / Admin";
                        doc.CurrentStatus = 3; // Completed
                        doc.DateCompleted = DateTime.Now;
                        newStatusText = "Approved";
                        validTransition = true;
                    }
                    break;

                case "RETURN":
                    // Go back to step 1
                    doc.CurrentStepIndex = 1;
                    doc.CurrentOfficeName = "Receiving";
                    doc.CurrentStatus = 2; // Returned
                    newStatusText = "Returned to Sender";
                    validTransition = true;
                    break;

                case "REJECT":
                    doc.CurrentStatus = 4; // Rejected (Terminal state)
                    doc.CurrentOfficeName = "Rejected";
                    newStatusText = "Rejected";
                    validTransition = true;
                    break;

                case "REASSIGN":
                    if (targetUserId.HasValue) {
                        doc.AssignedToUserId = targetUserId;
                        var targetUser = _context.Users.Find(targetUserId.Value);
                        newStatusText = $"Reassigned to {targetUser?.FullName}";
                        validTransition = true;
                    }
                    break;
            }

            if (!validTransition) return false;

            doc.LastUpdated = DateTime.Now;

            // Log strictly append-only audit trail
            _context.AuditTrailEntries.Add(new AuditTrailEntry
            {
                DocumentId = doc.Id,
                UserId = user.Id,
                Action = newStatusText,
                Details = string.IsNullOrWhiteSpace(remarks) ? $"System FSM transition executed by {user.FullName}" : remarks,
                Timestamp = DateTime.Now,
                Ipaddress = "System"
            });

            _context.WorkflowTransitions.Add(new WorkflowTransition
            {
                DocumentId = doc.Id,
                FromStatus = oldStep,
                ToStatus = doc.CurrentStepIndex,
                FromOffice = oldOffice,
                ToOffice = doc.CurrentOfficeName,
                ActionTaken = action,
                Remarks = remarks,
                PerformedByUserId = user.Id,
                TransitionDate = DateTime.Now,
                StepNumber = doc.CurrentStepIndex
            });

            if (action.ToUpper() == "APPROVE" && !string.IsNullOrEmpty(signatureData))
            {
                _context.DigitalSignatures.Add(new DigitalSignature
                {
                    DocumentId = doc.Id,
                    SignedByUserId = user.Id,
                    SignatureImagePath = signatureData,
                    SignedAt = DateTime.Now,
                    SignatureHash = "NO_HASH", // TODO: Implement proper hash
                    ActionType = "Approved",
                    IsVerified = true
                });
            }

            _context.SaveChanges();

            // Trigger SMS Notifications for state changes
            if (!string.IsNullOrEmpty(doc.ContactNumber))
            {
                if (action.ToUpper() == "APPROVE" || action.ToUpper() == "REJECT" || action.ToUpper() == "RETURN")
                {
                    _sms.Send(doc.Id, doc.ContactNumber, doc.SubmittedBy, 
                        $"TrackNGo Mati: Your document '{doc.Title}' ({doc.TrackingNumber}) status is now: {newStatusText}.", newStatusText);
                }
                else if (action.ToUpper() == "ENDORSE" || action.ToUpper() == "ACCEPT" || action.ToUpper() == "REVIEW")
                {
                    _sms.Send(doc.Id, doc.ContactNumber, doc.SubmittedBy, 
                        $"TrackNGo Mati: Your document ({doc.TrackingNumber}) was {newStatusText}.", newStatusText);
                }
            }

            return true;
        }
    }
}
