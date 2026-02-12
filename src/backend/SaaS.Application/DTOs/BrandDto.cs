namespace SaaS.Application.DTOs;

public class BrandDto
{
    public Guid Id { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string PrimaryColor { get; set; } = string.Empty;
    public string SecondaryColor { get; set; } = string.Empty;
    public string Slogan { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? PrivacyPolicy { get; set; }
    public string? TermsOfService { get; set; }
    public string BuiltBy { get; set; } = string.Empty;
    public string ConfigJson { get; set; } = "{}";
    public string? Port { get; set; }
    public string PortalUrl { get; set; } = string.Empty;
    public int SortOrder { get; set; } = 999;
    public bool IsActive { get; set; } = true;
}
