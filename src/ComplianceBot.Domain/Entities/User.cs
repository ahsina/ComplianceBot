using ComplianceBot.Domain.Common;
using ComplianceBot.Domain.Enums;

namespace ComplianceBot.Domain.Entities;

/// <summary>
/// User entity - represents a user within a tenant organization
/// </summary>
public class User : TenantEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// User role within the tenant
    /// </summary>
    public UserRole Role { get; set; }

    /// <summary>
    /// Authentication identifier (from Identity Service)
    /// </summary>
    public string IdentityId { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Full name for display
    /// </summary>
    public string FullName => $"{FirstName} {LastName}";

    // Navigation property
    public Tenant? Tenant { get; set; }
}
