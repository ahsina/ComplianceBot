using ComplianceBot.Domain.Common;
using ComplianceBot.Domain.Enums;

namespace ComplianceBot.Domain.Entities;

/// <summary>
/// Tenant entity - represents a company/organization using the platform
/// Multi-tenancy root entity
/// </summary>
public class Tenant : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string CompanyRegistrationNumber { get; set; } = string.Empty;
    public string VATNumber { get; set; } = string.Empty;

    public string Country { get; set; } = "LU"; // Default Luxembourg
    public string City { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;

    public string PrimaryContactName { get; set; } = string.Empty;
    public string PrimaryContactEmail { get; set; } = string.Empty;
    public string PrimaryContactPhone { get; set; } = string.Empty;

    /// <summary>
    /// Subscription information
    /// </summary>
    public SubscriptionTier SubscriptionTier { get; set; }
    public DateTime SubscriptionStartDate { get; set; }
    public DateTime? SubscriptionEndDate { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Enabled report types for this tenant based on subscription
    /// </summary>
    public List<ReportType> EnabledReportTypes { get; set; } = new();

    /// <summary>
    /// Database schema name for tenant isolation
    /// Format: tenant_{TenantId}
    /// </summary>
    public string DatabaseSchema => $"tenant_{Id:N}";

    // Navigation properties
    public ICollection<User> Users { get; set; } = new List<User>();
}
