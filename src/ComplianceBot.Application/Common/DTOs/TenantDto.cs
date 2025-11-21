using ComplianceBot.Domain.Enums;

namespace ComplianceBot.Application.Common.DTOs;

/// <summary>
/// Data transfer object for Tenant
/// </summary>
public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CompanyRegistrationNumber { get; set; } = string.Empty;
    public string VATNumber { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string PrimaryContactName { get; set; } = string.Empty;
    public string PrimaryContactEmail { get; set; } = string.Empty;
    public string PrimaryContactPhone { get; set; } = string.Empty;
    public SubscriptionTier SubscriptionTier { get; set; }
    public DateTime SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    public bool IsActive { get; set; }
    public List<ReportType> EnabledReportTypes { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
