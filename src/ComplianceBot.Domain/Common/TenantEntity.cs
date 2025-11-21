namespace ComplianceBot.Domain.Common;

/// <summary>
/// Base entity for all tenant-scoped entities
/// Ensures multi-tenancy isolation at the domain level
/// </summary>
public abstract class TenantEntity : AuditableEntity
{
    /// <summary>
    /// Tenant identifier for multi-tenancy isolation
    /// </summary>
    public Guid TenantId { get; set; }
}
