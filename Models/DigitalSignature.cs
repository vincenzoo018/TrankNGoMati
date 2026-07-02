using System;
using System.Collections.Generic;

namespace TrackNGoMati.Models;

public partial class DigitalSignature
{
    public int Id { get; set; }

    public int DocumentId { get; set; }

    public int SignedByUserId { get; set; }

    public string SignatureImagePath { get; set; } = null!;

    public string SignatureHash { get; set; } = null!;

    public string ActionType { get; set; } = null!;

    public string? Remarks { get; set; }

    public DateTime SignedAt { get; set; }

    public bool IsVerified { get; set; }

    public virtual Document Document { get; set; } = null!;

    public virtual User SignedByUser { get; set; } = null!;
}
