using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class WorkflowStep
{
    public int Id { get; set; }

    public int TypeId { get; set; }

    public int StepNumber { get; set; }

    public string StepName { get; set; } = null!;

    public int? AssignedDepartmentId { get; set; }

    public string AllowedActions { get; set; } = null!;

    public virtual Department? AssignedDepartment { get; set; }

    public virtual DocumentTypeConfig Type { get; set; } = null!;
}
