using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class RoutingSlip
{
    public int Id { get; set; }

    public int DocumentId { get; set; }

    public string TrackingNumber { get; set; } = null!;

    public int ReceivedByUserId { get; set; }

    public DateTime DateReceived { get; set; }

    public string SenderName { get; set; } = null!;

    public string? ActionInstruction { get; set; }

    public int? TargetDepartmentId { get; set; }

    public int? NotedByUserId { get; set; }

    public string SlipStatus { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Document Document { get; set; } = null!;

    public virtual User? NotedByUser { get; set; }

    public virtual User ReceivedByUser { get; set; } = null!;

    public virtual Department? TargetDepartment { get; set; }
}
