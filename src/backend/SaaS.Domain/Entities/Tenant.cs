namespace SaaS.Domain.Entities;

using SaaS.Domain.Common;

public class Tenant : BaseEntity<string>
{
    public string Name { get; set; } = string.Empty;
    public string Hostname { get; set; } = string.Empty; // e.g., greenpantry.amozona.in
    public bool IsActive { get; set; } = true;
    public string Identifier {get; set; } = "";
    public string? AllowedOrigins { get; set; } // Comma-separated list for CORS validation
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual Brand? Brand { get; set; }
}

public class Brand : BaseEntity<Guid>, ITenantEntity
{
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = "#000000";
    public string SecondaryColor { get; set; } = "#ffffff";
    public string Slogan { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? PrivacyPolicy { get; set; }
    public string? TermsOfService { get; set; }
    public string BuiltBy { get; set; } = string.Empty;
    public string ConfigJson { get; set; } = "{}"; // For feature flags and custom UI config
    public int SortOrder { get; set; } = 999; // For custom ordering in dashboard
    public bool IsVisible { get; set; } = true; // Control visibility in dashboard
    public bool IsActive { get; set; } = true; // Control functionality status
    public string? Port { get; set; } // For local development redirect
}

