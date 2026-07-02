using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class Smsnotification
{
    public int Id { get; set; }

    public int DocumentId { get; set; }

    public int? RecipientUserId { get; set; }

    public string RecipientNumber { get; set; } = null!;

    public string RecipientName { get; set; } = null!;

    public string MessageContent { get; set; } = null!;

    public string TriggerEvent { get; set; } = null!;

    /// <summary>0=Queued, 1=Delivered, 2=Failed</summary>
    public int Status { get; set; }

    /// <summary>Convenience bool alias for Status == 1</summary>
    public bool IsDelivered
    {
        get => Status == 1;
        set => Status = value ? 1 : 0;
    }

    public DateTime QueuedAt { get; set; }

    public DateTime? SentAt { get; set; }

    public string? GatewayResponse { get; set; }

    public virtual Document Document { get; set; } = null!;

    public virtual User? RecipientUser { get; set; }
}
