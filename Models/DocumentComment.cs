using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class DocumentComment
{
    public int Id { get; set; }

    public int DocumentId { get; set; }

    public int UserId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime PostedAt { get; set; }

    public bool IsInternal { get; set; }

    public string RemarkType { get; set; } = null!;

    public string? AnchorLocation { get; set; }

    public string? WorkflowState { get; set; }

    public bool Resolved { get; set; }

    public virtual Document Document { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
