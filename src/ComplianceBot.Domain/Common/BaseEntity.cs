namespace ComplianceBot.Domain.Common;

/// <summary>
/// Base entity for all domain entities with primary key
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; }

    /// <summary>
    /// Domain events collection for DDD event handling
    /// </summary>
    private readonly List<IDomainEvent> _domainEvents = new();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
