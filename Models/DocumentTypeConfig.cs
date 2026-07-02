using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class DocumentTypeConfig
{
    public int Id { get; set; }

    public string TypeName { get; set; } = null!;

    public int TotalSteps { get; set; }

    /// <summary>ARTA processing days: 3 (Simple), 7 (Complex), 20 (Highly Technical)</summary>
    public int DefaultProcessingDays { get; set; } = 3;

    public string? Description { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<WorkflowStep> WorkflowSteps { get; set; } = new List<WorkflowStep>();
}
