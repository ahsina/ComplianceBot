namespace ComplianceBot.Domain.Common;

/// <summary>
/// Base entity with audit tracking capabilities
/// Tracks who created/modified the entity and when
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }
    public string? ModifiedBy { get; set; }
}
