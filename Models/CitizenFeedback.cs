using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TrackNGoMati.Models;

public class CitizenFeedback
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string TrackingNumber { get; set; } = null!;

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    [StringLength(1000)]
    public string? Comments { get; set; }

    public DateTime DateSubmitted { get; set; }
}
