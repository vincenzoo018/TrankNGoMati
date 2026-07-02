using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class Department
{
    public int Id { get; set; }

    public string DepartmentName { get; set; } = null!;

    public string DepartmentCode { get; set; } = null!;

    public int? HeadUserId { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual User? HeadUser { get; set; }

    public virtual ICollection<RoutingSlip> RoutingSlips { get; set; } = new List<RoutingSlip>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<WorkflowStep> WorkflowSteps { get; set; } = new List<WorkflowStep>();
}
