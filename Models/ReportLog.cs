using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class ReportLog
{
    public int Id { get; set; }

    public int GeneratedByUserId { get; set; }

    public string ReportType { get; set; } = null!;

    public DateTime DateFrom { get; set; }

    public DateTime DateTo { get; set; }

    public DateTime GeneratedAt { get; set; }

    public virtual User GeneratedByUser { get; set; } = null!;
}
