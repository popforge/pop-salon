// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate

using FluentValidation;
using MediatR;
using Popsalon.Domain.Entities;
using Popsalon.Domain.Interfaces;

namespace Popsalon.Application.Features.Appointments.Commands.Create;

public record CreateAppointmentCommand(
    DateTime Date,
    Guid CustomerId,
    string? Notes
) : IRequest<Guid>;

public class CreateAppointmentCommandValidator : AbstractValidator<CreateAppointmentCommand>
{
    public CreateAppointmentCommandValidator()
    {
        RuleFor(x => x.Date)
            .GreaterThan(DateTime.UtcNow).WithMessage("La date doit être dans le futur.");

        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Un client est requis.");
    }
}

public class CreateAppointmentCommandHandler(IAppointmentRepository repository)
    : IRequestHandler<CreateAppointmentCommand, Guid>
{
    public async Task<Guid> Handle(CreateAppointmentCommand request, CancellationToken ct)
    {
        var appointment = new Appointment(request.Date, request.CustomerId, request.Notes);
        await repository.AddAsync(appointment, ct);
        return appointment.Id;
    }
}
