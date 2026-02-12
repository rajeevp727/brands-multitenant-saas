using System;

namespace SaaS.Application.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid? TenantId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string TargetRole { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TimeAgo { get; set; } = string.Empty; // Helper for UI
    }
}
