// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate
// Source : metadata/entities/Appointment.yml

using MediatR;
using Popsalon.Application.EntityViews;

namespace Popsalon.Application.Features.Appointments.Queries.GetAll;

public record GetAllAppointmentsQuery : IRequest<IQueryable<AppointmentView>>;
