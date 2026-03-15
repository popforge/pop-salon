// FICHIER GÉNÉRÉ AUTOMATIQUEMENT — forge generate

using MediatR;
using Microsoft.EntityFrameworkCore;
using Popsalon.Application.Common.Exceptions;
using Popsalon.Application.Common.Interfaces;
using Popsalon.Application.EntityViews;
using Popsalon.Domain.Entities;

namespace Popsalon.Application.Features.Appointments.Queries.GetById;

public record GetAppointmentByIdQuery(Guid Id) : IRequest<AppointmentView>;

public class GetAppointmentByIdQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetAppointmentByIdQuery, AppointmentView>
{
    public async Task<AppointmentView> Handle(GetAppointmentByIdQuery request, CancellationToken ct)
    {
        var view = await db.Appointments
            .AsNoTracking()
            .Where(a => a.Id == request.Id)
            .Select(a => new AppointmentView
            {
                Id = a.Id,
                Date = a.Date,
                Notes = a.Notes,
                CustomerId = a.CustomerId,
                CustomerFullName = a.Customer.FirstName + " " + a.Customer.LastName,
                CustomerEmail = a.Customer.Email,
            })
            .FirstOrDefaultAsync(ct);

        return view ?? throw new NotFoundException(nameof(Appointment), request.Id);
    }
}
