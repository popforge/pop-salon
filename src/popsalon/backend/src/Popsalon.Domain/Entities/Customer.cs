using Popsalon.Domain.Entities;
using Popsalon.Domain.Events;
using Popsalon.Domain.Exceptions;

namespace Popsalon.Domain.Entities;

public class Customer : BusinessEntity<Guid>
{
    public string FirstName { get; private set; } = "";
    public string LastName { get; private set; } = "";
    public string? Email { get; private set; }
    public string? Phone { get; private set; }

    // Navigation EF Core
    public ICollection<Appointment> Appointments { get; private set; } = [];

    private Customer() { } // Constructeur EF Core

    public Customer(string firstName, string lastName, string? email = null, string? phone = null)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new DomainValidationException("Le prénom est requis.");
        if (string.IsNullOrWhiteSpace(lastName)) throw new DomainValidationException("Le nom est requis.");

        Id = Guid.NewGuid();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email?.Trim().ToLowerInvariant();
        Phone = phone?.Trim();

        AddDomainEvent(new CustomerCreatedEvent(Id, FirstName, LastName));
    }

    public void Update(string firstName, string lastName, string? email, string? phone)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new DomainValidationException("Le prénom est requis.");
        if (string.IsNullOrWhiteSpace(lastName)) throw new DomainValidationException("Le nom est requis.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Email = email?.Trim().ToLowerInvariant();
        Phone = phone?.Trim();
    }

    public string FullName => $"{FirstName} {LastName}";
}

// Domain event
public record CustomerCreatedEvent(Guid CustomerId, string FirstName, string LastName) : IDomainEvent
{
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
}
