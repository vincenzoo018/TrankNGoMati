using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class DocumentAttachment
{
    public int Id { get; set; }

    public int DocumentId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public long FileSize { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual Document Document { get; set; } = null!;
}
