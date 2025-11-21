namespace ComplianceBot.Domain.Enums;

/// <summary>
/// User roles within a tenant organization
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Tenant administrator - full access
    /// </summary>
    TenantAdmin = 1,

    /// <summary>
    /// Compliance officer - can create and submit reports
    /// </summary>
    ComplianceOfficer = 2,

    /// <summary>
    /// Compliance analyst - can create and validate reports
    /// </summary>
    ComplianceAnalyst = 3,

    /// <summary>
    /// Viewer - read-only access
    /// </summary>
    Viewer = 4,

    /// <summary>
    /// System administrator - platform-wide access (ComplianceBot staff)
    /// </summary>
    SystemAdmin = 100
}
