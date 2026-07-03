using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class DocumentMetadata
{
    public int Id { get; set; }

    public int DocumentId { get; set; }

    public string? ConferenceName { get; set; }

    public string? SourceLink { get; set; }

    public string? Province { get; set; }

    public string? ReportNumber { get; set; }

    public string? CategoryFlags { get; set; }

    public string? ExtractedText { get; set; }

    public virtual Document Document { get; set; } = null!;
}
