using ComplianceBot.Application.Common.Interfaces;

namespace ComplianceBot.Infrastructure.Identity;

/// <summary>
/// Implementation of ITenantContext
/// Provides current tenant information from HTTP context
/// </summary>
public class TenantContext : ITenantContext
{
    private Guid _tenantId;

    public Guid TenantId => _tenantId;

    public string TenantSchema => $"tenant_{_tenantId:N}";

    public bool IsSystemAdmin { get; private set; }

    public void SetTenant(Guid tenantId)
    {
        _tenantId = tenantId;
    }

    public void SetSystemAdmin(bool isSystemAdmin)
    {
        IsSystemAdmin = isSystemAdmin;
    }
}
