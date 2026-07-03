using System;

namespace TrackNGoMati.Models;

public partial class DocumentTemplate
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Category { get; set; } = null!;

    public int CreatedByUserId { get; set; }
    
    public DateTime CreatedAt { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;
}
