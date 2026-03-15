// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate
// Source : metadata/entities/Customer.yml

namespace Popsalon.Application.EntityViews;

/// <summary>Modèle de lecture (CQRS read side) pour l'entité Customer.</summary>
public sealed record CustomerView
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = "";
    public string LastName { get; init; } = "";
    public string FullName { get; init; } = "";
    public string? Email { get; init; }
    public string? Phone { get; init; }
}
