using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class Document
{
    public int Id { get; set; }

    public string TrackingNumber { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string DocumentType { get; set; } = null!;

    public int? TypeId { get; set; }

    public string OriginatingDepartment { get; set; } = null!;

    public int? DepartmentId { get; set; }

    public string SubmittedBy { get; set; } = null!;

    public int? SubmittedByUserId { get; set; }

    public int? AssignedToUserId { get; set; }

    public string? ContactNumber { get; set; }

    public string? EmailAddress { get; set; }

    public int CurrentStatus { get; set; }

    public string CurrentOfficeName { get; set; } = null!;

    public int CurrentStepIndex { get; set; }

    public int TotalSteps { get; set; }

    public string? AttachmentPath { get; set; }

    public string? QrcodePath { get; set; }

    public DateTime DateFiled { get; set; }

    public DateTime? DateCompleted { get; set; }

    public DateTime LastUpdated { get; set; }

    public int ArtaprocessingDays { get; set; }

    public DateTime? EscalationDeadline { get; set; }

    public bool IsEscalated { get; set; }

    public bool IsLocked { get; set; }

    public int CreatedByUserId { get; set; }

    public bool IsUrgent { get; set; }

    public string? UrgencyJustification { get; set; }

    public int? ParentDocumentId { get; set; }

    public int VersionNumber { get; set; }

    public virtual ICollection<AuditTrailEntry> AuditTrailEntries { get; set; } = new List<AuditTrailEntry>();

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual User? AssignedToUser { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<DigitalSignature> DigitalSignatures { get; set; } = new List<DigitalSignature>();

    public virtual ICollection<DocumentAttachment> DocumentAttachments { get; set; } = new List<DocumentAttachment>();

    public virtual ICollection<DocumentComment> DocumentComments { get; set; } = new List<DocumentComment>();

    public virtual DocumentMetadata? DocumentMetadata { get; set; }

    public virtual ICollection<EscalationLog> EscalationLogs { get; set; } = new List<EscalationLog>();

    public virtual ICollection<QrcodeRecord> QrcodeRecords { get; set; } = new List<QrcodeRecord>();

    public virtual RoutingSlip? RoutingSlip { get; set; }

    public virtual ICollection<Smsnotification> Smsnotifications { get; set; } = new List<Smsnotification>();

    public virtual User? SubmittedByUser { get; set; }

    public virtual DocumentTypeConfig? Type { get; set; }

    public virtual ICollection<WorkflowTransition> WorkflowTransitions { get; set; } = new List<WorkflowTransition>();
}
