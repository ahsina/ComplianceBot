namespace ComplianceBot.Application.Common.Interfaces;

/// <summary>
/// Provides current tenant context for multi-tenancy
/// Injected by middleware, used throughout the application
/// </summary>
public interface ITenantContext
{
    Guid TenantId { get; }
    string TenantSchema { get; }
    bool IsSystemAdmin { get; }
    void SetTenant(Guid tenantId);
}
