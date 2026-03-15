using Popsalon.Domain.Events;

namespace Popsalon.Domain.Entities;

/// <summary>
/// Classe de base pour toutes les entités du domaine.
/// Fournit l'identifiant typé, les dates d'audit et les domain events.
/// </summary>
public abstract class BusinessEntity<TId>
{
    public TId Id { get; protected set; } = default!;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents() => _domainEvents.Clear();

    public void SetUpdatedAt(DateTime at) => UpdatedAt = at;
}
