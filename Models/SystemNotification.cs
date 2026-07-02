using System;
using System.ComponentModel.DataAnnotations;

namespace TrackNGoMati.Models
{
    public class SystemNotification
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        
        [Required]
        public string Title { get; set; } = null!;
        
        [Required]
        public string Message { get; set; } = null!;
        
        public string? ActionUrl { get; set; }
        
        public bool IsRead { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual User User { get; set; } = null!;
    }
}
