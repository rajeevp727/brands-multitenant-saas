using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaaS.Domain.Entities
{
    public class Notification
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid? TenantId { get; set; } // Nullable for Global System Notifications

        [Required]
        [MaxLength(100)]
        public string BrandName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string TargetRole { get; set; } = "All"; // Admin, Customer, Vendor, etc.

        public Guid? UserId { get; set; } // Nullable for broadcast messages

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = "Info"; // Order, System, Alert

        [Required]
        [MaxLength(20)]
        public string Severity { get; set; } = "Info"; // Info, Warning, Critical

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
