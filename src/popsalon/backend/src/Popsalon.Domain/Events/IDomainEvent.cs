namespace Popsalon.Domain.Events;

/// <summary>Marqueur pour tous les événements de domaine.</summary>
public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}
