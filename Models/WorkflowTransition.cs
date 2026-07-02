using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class WorkflowTransition
{
    public int Id { get; set; }

    public int DocumentId { get; set; }

    public int FromStatus { get; set; }

    public int ToStatus { get; set; }

    public string FromOffice { get; set; } = null!;

    public string ToOffice { get; set; } = null!;

    public string ActionTaken { get; set; } = null!;

    public string? Remarks { get; set; }

    public int PerformedByUserId { get; set; }

    public DateTime TransitionDate { get; set; }

    public int StepNumber { get; set; }

    public virtual Document Document { get; set; } = null!;

    public virtual User PerformedByUser { get; set; } = null!;
}
