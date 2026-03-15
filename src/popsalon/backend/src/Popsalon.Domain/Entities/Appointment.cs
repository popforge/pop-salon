using Popsalon.Domain.Events;
using Popsalon.Domain.Exceptions;

namespace Popsalon.Domain.Entities;

public class Appointment : BusinessEntity<Guid>
{
    public DateTime Date { get; private set; }
    public string? Notes { get; private set; }
    public Guid CustomerId { get; private set; }

    // Navigation EF Core
    public Customer Customer { get; private set; } = null!;

    private Appointment() { } // Constructeur EF Core

    public Appointment(DateTime date, Guid customerId, string? notes = null)
    {
        if (date <= DateTime.UtcNow)
            throw new DomainValidationException("La date du rendez-vous doit être dans le futur.");
        if (customerId == Guid.Empty)
            throw new DomainValidationException("Un client doit être associé au rendez-vous.");

        Id = Guid.NewGuid();
        Date = date;
        CustomerId = customerId;
        Notes = notes?.Trim();

        AddDomainEvent(new AppointmentCreatedEvent(Id, Date, CustomerId));
    }

    public void Reschedule(DateTime newDate)
    {
        if (newDate <= DateTime.UtcNow)
            throw new DomainValidationException("La nouvelle date doit être dans le futur.");

        Date = newDate;
        AddDomainEvent(new AppointmentRescheduledEvent(Id, Date, newDate));
    }

    public void UpdateNotes(string? notes) => Notes = notes?.Trim();
}

// Domain events
public record AppointmentCreatedEvent(Guid AppointmentId, DateTime Date, Guid CustomerId) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}

public record AppointmentRescheduledEvent(Guid AppointmentId, DateTime OldDate, DateTime NewDate) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
