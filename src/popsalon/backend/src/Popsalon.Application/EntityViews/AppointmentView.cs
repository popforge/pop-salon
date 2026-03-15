// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate
// Source : metadata/entities/Appointment.yml
// Pour modifier la structure, éditez le YAML puis relancez : forge generate

namespace Popsalon.Application.EntityViews;

/// <summary>Modèle de lecture (CQRS read side) pour l'entité Appointment.</summary>
public sealed record AppointmentView
{
    public Guid Id { get; init; }
    public DateTime Date { get; init; }
    public string? Notes { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerFullName { get; init; } = "";
    public string? CustomerEmail { get; init; }
}
