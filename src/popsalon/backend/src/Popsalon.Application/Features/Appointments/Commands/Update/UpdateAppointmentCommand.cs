// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate

using FluentValidation;
using MediatR;
using Popsalon.Application.Common.Exceptions;
using Popsalon.Domain.Entities;
using Popsalon.Domain.Interfaces;

namespace Popsalon.Application.Features.Appointments.Commands.Update;

public record UpdateAppointmentCommand(
    Guid Id,
    DateTime Date,
    string? Notes
) : IRequest;

public class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
{
    public UpdateAppointmentCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Date)
            .GreaterThan(DateTime.UtcNow).WithMessage("La date doit être dans le futur.");
    }
}

public class UpdateAppointmentCommandHandler(IAppointmentRepository repository)
    : IRequestHandler<UpdateAppointmentCommand>
{
    public async Task Handle(UpdateAppointmentCommand request, CancellationToken ct)
    {
        var appointment = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Appointment), request.Id);

        appointment.Reschedule(request.Date);
        appointment.UpdateNotes(request.Notes);
        await repository.UpdateAsync(appointment, ct);
    }
}
