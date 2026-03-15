// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate

using MediatR;
using Popsalon.Application.Common.Exceptions;
using Popsalon.Domain.Entities;
using Popsalon.Domain.Interfaces;

namespace Popsalon.Application.Features.Appointments.Commands.Delete;

public record DeleteAppointmentCommand(Guid Id) : IRequest;

public class DeleteAppointmentCommandHandler(IAppointmentRepository repository)
    : IRequestHandler<DeleteAppointmentCommand>
{
    public async Task Handle(DeleteAppointmentCommand request, CancellationToken ct)
    {
        var appointment = await repository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Appointment), request.Id);

        await repository.DeleteAsync(appointment, ct);
    }
}
