using System;

namespace GreenPantry.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? TenantId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string TargetRole { get; set; } = "All";
        public Guid? UserId { get; set; }
        public string Type { get; set; } = "Info";
        public string Severity { get; set; } = "Info";
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
