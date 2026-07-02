using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class EscalationLog
{
    public int Id { get; set; }

    public int DocumentId { get; set; }

    public string ViolatingOffice { get; set; } = null!;

    public int Artathreshold { get; set; }

    public int ArtaperiodDays { get; set; }

    public int ElapsedDays { get; set; }

    // Legacy alias kept for any existing DB columns
    public int ActualElapsedDays { get => ElapsedDays; set => ElapsedDays = value; }

    public string EscalationLevel { get; set; } = null!;

    public int? NotifiedUserId { get; set; }

    public bool NotificationSent { get; set; }

    public string EscalationReason { get; set; } = null!;

    public DateTime EscalatedAt { get; set; }

    public string? ResolutionNotes { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public bool IsResolved { get; set; }

    public bool Resolved { get => IsResolved; set => IsResolved = value; }

    public int? ResolvedByUserId { get; set; }

    public virtual Document Document { get; set; } = null!;

    public virtual User? NotifiedUser { get; set; }

    public virtual User? ResolvedByUser { get; set; }
}
