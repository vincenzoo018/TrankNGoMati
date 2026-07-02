using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class QrcodeRecord
{
    public int Id { get; set; }

    public int DocumentId { get; set; }

    public string TrackingNumber { get; set; } = null!;

    public string QrcodeImagePath { get; set; } = null!;

    public string TrackingUrl { get; set; } = null!;

    public DateTime GeneratedAt { get; set; }

    public int ScanCount { get; set; }

    public DateTime? LastScannedAt { get; set; }

    public virtual Document Document { get; set; } = null!;
}
