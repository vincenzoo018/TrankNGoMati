using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class AuditTrailEntry
{
    public int Id { get; set; }

    public int? DocumentId { get; set; }

    public int? UserId { get; set; }

    public string Action { get; set; } = null!;

    public string Details { get; set; } = null!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? Ipaddress { get; set; }

    public DateTime Timestamp { get; set; }

    public virtual Document? Document { get; set; }

    public virtual User? User { get; set; }
}
