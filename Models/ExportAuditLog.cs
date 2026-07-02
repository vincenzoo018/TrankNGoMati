using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class ExportAuditLog
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string ExportType { get; set; } = null!;

    public string ExportScope { get; set; } = null!;

    public int RecordCount { get; set; }

    public DateTime ExportedAt { get; set; }

    public string? Ipaddress { get; set; }

    public virtual User User { get; set; } = null!;
}
