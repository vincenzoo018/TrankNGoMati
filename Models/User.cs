using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class User
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? MobileNumber { get; set; }

    public int Role { get; set; }

    public string Department { get; set; } = null!;

    public int? DepartmentId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public string? ProfileImagePath { get; set; }

    public string? ExportPasswordHash { get; set; }

    public virtual ICollection<AuditTrailEntry> AuditTrailEntries { get; set; } = new List<AuditTrailEntry>();

    public virtual Department? DepartmentNavigation { get; set; }

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<DigitalSignature> DigitalSignatures { get; set; } = new List<DigitalSignature>();

    public virtual ICollection<DocumentComment> DocumentComments { get; set; } = new List<DocumentComment>();

    public virtual ICollection<Document> DocumentCreatedByUsers { get; set; } = new List<Document>();

    public virtual ICollection<Document> DocumentSubmittedByUsers { get; set; } = new List<Document>();

    public virtual ICollection<EscalationLog> EscalationLogNotifiedUsers { get; set; } = new List<EscalationLog>();

    public virtual ICollection<EscalationLog> EscalationLogResolvedByUsers { get; set; } = new List<EscalationLog>();

    public virtual ICollection<ExportAuditLog> ExportAuditLogs { get; set; } = new List<ExportAuditLog>();

    public virtual ICollection<ReportLog> ReportLogs { get; set; } = new List<ReportLog>();

    public virtual ICollection<RoutingSlip> RoutingSlipNotedByUsers { get; set; } = new List<RoutingSlip>();

    public virtual ICollection<RoutingSlip> RoutingSlipReceivedByUsers { get; set; } = new List<RoutingSlip>();

    public virtual ICollection<Smsnotification> Smsnotifications { get; set; } = new List<Smsnotification>();

    public virtual ICollection<WorkflowTransition> WorkflowTransitions { get; set; } = new List<WorkflowTransition>();
}
